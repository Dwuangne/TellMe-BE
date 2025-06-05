# TellMe ChatHub - Hướng dẫn sử dụng

## Tổng quan

`ChatHub` đã được tối ưu hóa để hỗ trợ chat realtime với các tính năng:

- **Kết nối tự động**: Người dùng tự động join vào các cuộc hội thoại hiện có khi kết nối
- **Chat 1-1**: Dễ dàng bắt đầu cuộc hội thoại mới với người khác
- **Realtime messaging**: Tin nhắn được gửi ngay lập tức đến tất cả participants
- **Typing indicators**: Hiển thị khi người khác đang typing
- **Connection tracking**: Theo dõi users online/offline
- **Authentication**: Bảo mật với JWT tokens

## Cấu hình

### 1. Backend Setup

Đảm bảo các services đã được đăng ký trong `DependencyExtension.cs`:

```csharp
// Chat Services
services.AddScoped<IConversationService, ConversationService>();
services.AddScoped<IMessageService, MessageService>();
services.AddScoped<IParticipantService, ParticipantService>();

// SignalR
services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});
```

### 2. CORS Configuration

```csharp
services.AddCors(cors => cors.AddPolicy(
    name: CorsConstant.PolicyName,
    policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Quan trọng cho SignalR
    }));
```

### 3. JWT for SignalR

```csharp
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;
        
        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
        {
            context.Token = accessToken;
        }
        return Task.CompletedTask;
    }
};
```

## Client-side Integration

### 1. Kết nối với JWT

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub", {
        accessTokenFactory: () => yourJwtToken
    })
    .build();

await connection.start();
```

### 2. Event Handlers

```javascript
// Khi kết nối thành công
connection.on("Connected", function (userId) {
    console.log('Connected as user:', userId);
});

// Nhận tin nhắn
connection.on("ReceiveMessage", function (message) {
    displayMessage(message);
});

// Cuộc hội thoại mới
connection.on("NewConversationCreated", function (conversation) {
    addConversationToList(conversation);
});

// Typing indicator
connection.on("UserTyping", function (userId, isTyping) {
    showTypingIndicator(userId, isTyping);
});

// Error handling
connection.on("Error", function (message) {
    console.error('Hub error:', message);
});
```

## Hub Methods

### 1. Bắt đầu cuộc hội thoại mới

```javascript
// Bắt đầu chat với user khác
await connection.invoke("StartConversationWithUser", targetUserId, "Hello!");
```

**Server method:**
```csharp
public async Task StartConversationWithUser(Guid targetUserId, string content)
```

### 2. Gửi tin nhắn

```javascript
// Gửi tin nhắn trong cuộc hội thoại
await connection.invoke("SendMessage", conversationId, "Tin nhắn của tôi", currentUserId);
```

**Server method:**
```csharp
public async Task SendMessage(Guid conversationId, string content, Guid userId)
```

### 3. Join cuộc hội thoại

```javascript
// Join vào cuộc hội thoại cụ thể
await connection.invoke("JoinConversation", conversationId);
```

**Server method:**
```csharp
public async Task JoinConversation(Guid conversationId)
```

### 4. Lấy danh sách cuộc hội thoại

```javascript
// Lấy cuộc hội thoại của user hiện tại
await connection.invoke("GetMyConversations");
```

**Server method:**
```csharp
public async Task GetMyConversations()
```

### 5. Typing indicator

```javascript
// Báo hiệu đang typing
await connection.invoke("UserTyping", conversationId, true);

// Ngừng typing
await connection.invoke("UserTyping", conversationId, false);
```

**Server method:**
```csharp
public async Task UserTyping(Guid conversationId, bool isTyping)
```

### 6. Đánh dấu đã đọc

```javascript
// Đánh dấu tin nhắn đã đọc
await connection.invoke("MarkMessagesAsRead", conversationId);
```

**Server method:**
```csharp
public async Task MarkMessagesAsRead(Guid conversationId)
```

## Client Events

### Nhận từ server

| Event | Mô tả | Parameters |
|-------|-------|------------|
| `Connected` | Kết nối thành công | `userId` |
| `ReceiveMessage` | Nhận tin nhắn mới | `message` object |
| `NewConversationCreated` | Cuộc hội thoại mới được tạo | `conversation` object |
| `UserJoinedConversation` | User join vào conversation | `userId`, `userInfo` |
| `JoinedConversation` | Bạn đã join conversation | `conversationId` |
| `MyConversations` | Danh sách cuộc hội thoại | `conversations` array |
| `UserTyping` | User đang/ngừng typing | `userId`, `isTyping` |
| `MessagesRead` | Tin nhắn đã được đọc | `userId`, `conversationId` |
| `Error` | Lỗi xảy ra | `errorMessage` |

## Data Models

### Message Object
```json
{
    "id": "guid",
    "content": "string",
    "sendAt": "datetime",
    "conversationId": "guid", 
    "userId": "guid"
}
```

### Conversation Object
```json
{
    "id": "guid",
    "name": "string",
    "createdAt": "datetime",
    "participants": [
        {
            "id": "guid",
            "userId": "guid",
            "conversationId": "guid",
            "joinedAt": "datetime",
            "fullName": "string"
        }
    ],
    "messages": []
}
```

## Ví dụ Full Flow

### 1. Kết nối và Setup

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub", {
        accessTokenFactory: () => localStorage.getItem('jwt_token')
    })
    .build();

// Setup event handlers
connection.on("Connected", (userId) => {
    console.log('Connected as:', userId);
    currentUserId = userId;
});

connection.on("ReceiveMessage", (message) => {
    addMessageToUI(message);
});

connection.on("NewConversationCreated", (conversation) => {
    addConversationToSidebar(conversation);
    selectConversation(conversation.id);
});

// Kết nối
await connection.start();
```

### 2. Bắt đầu chat mới

```javascript
async function startNewChat(targetUserId, initialMessage) {
    try {
        await connection.invoke("StartConversationWithUser", targetUserId, initialMessage);
    } catch (err) {
        console.error('Failed to start conversation:', err);
    }
}
```

### 3. Gửi tin nhắn

```javascript
async function sendMessage(conversationId, content) {
    try {
        await connection.invoke("SendMessage", conversationId, content, currentUserId);
        clearMessageInput();
    } catch (err) {
        console.error('Failed to send message:', err);
    }
}
```

### 4. Typing indicator

```javascript
let typingTimer;

function onMessageInputChange() {
    // User is typing
    connection.invoke("UserTyping", currentConversationId, true);
    
    // Stop typing after 3 seconds of inactivity
    clearTimeout(typingTimer);
    typingTimer = setTimeout(() => {
        connection.invoke("UserTyping", currentConversationId, false);
    }, 3000);
}
```

## Error Handling

Tất cả methods đều có error handling built-in và sẽ gửi event `Error` nếu có lỗi:

```javascript
connection.on("Error", function (errorMessage) {
    showErrorNotification(errorMessage);
});
```

## Security Features

1. **JWT Authentication**: Tất cả connections đều cần JWT valid
2. **Authorization**: Users chỉ có thể join conversations mà họ là participant
3. **Input Validation**: Tất cả inputs đều được validate
4. **Exception Handling**: Errors được handle gracefully

## Performance Optimizations

1. **Connection Tracking**: Sử dụng `ConcurrentDictionary` để track connections
2. **Lazy Loading**: Messages chỉ được load khi cần
3. **Pagination**: Hỗ trợ pagination cho conversations và messages
4. **Auto-reconnection**: SignalR tự động reconnect khi mất kết nối

## Demo

Truy cập `/chat-demo.html` để xem demo đầy đủ các tính năng.

## Troubleshooting

### Connection Issues
- Kiểm tra JWT token có valid không
- Đảm bảo CORS được cấu hình đúng với `AllowCredentials()`
- Kiểm tra SignalR service đã được đăng ký

### Authentication Issues  
- Đảm bảo JWT events được setup cho SignalR
- Token phải được gửi qua query string hoặc header

### Message Not Delivered
- Kiểm tra user có là participant của conversation không
- Verify connection status
- Check server logs for errors 