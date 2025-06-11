using TellMe.Service.Services.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using TellMe.Service.Models.RequestModels;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using TellMe.Repository.Enities; // Added namespace for ApplicationUser

namespace TellMe.API.Hubs
{
    [Authorize] 
    public class ChatHub : Hub
    {
        private readonly IMessageService messageService;
        private readonly IConversationService conversationService;
        private readonly IParticipantService participantService;
        private readonly UserManager<ApplicationUser> userManager; // Changed from IdentityUser to ApplicationUser
        private readonly ILogger<ChatHub> logger;

        // Static dictionary to track user connections
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        // Static dictionary to track user's conversations
        private static readonly ConcurrentDictionary<string, HashSet<string>> UserConversations = new();

        public ChatHub(
            IMessageService messageService,
            IConversationService conversationService,
            IParticipantService participantService,
            UserManager<ApplicationUser> userManager, // Changed from IdentityUser to ApplicationUser
            ILogger<ChatHub> logger)
        {
            this.messageService = messageService;
            this.conversationService = conversationService;
            this.participantService = participantService;
            this.userManager = userManager;
            this.logger = logger;
        }

        // Override connection methods for better tracking
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (!string.IsNullOrEmpty(userId))
                {
                    UserConnections[userId] = Context.ConnectionId;

                    // Join user to their existing conversations
                    var userConversations = await GetUserConversationsAsync(Guid.Parse(userId));
                    var conversationIds = new HashSet<string>();

                    foreach (var conversation in userConversations)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
                        conversationIds.Add(conversation.Id.ToString());
                    }

                    UserConversations[userId] = conversationIds;

                    await Clients.Caller.SendAsync("Connected", userId);
                    logger.LogInformation($"User {userId} connected with connection {Context.ConnectionId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnConnectedAsync");
                await Clients.Caller.SendAsync("Error", "Failed to connect");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (!string.IsNullOrEmpty(userId))
                {
                    UserConnections.TryRemove(userId, out _);
                    UserConversations.TryRemove(userId, out _);

                    logger.LogInformation($"User {userId} disconnected");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnDisconnectedAsync");
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a specific conversation
        /// </summary>
        // Sửa đổi phương thức JoinConversation trong ChatHub.cs
        public async Task JoinConversation(Guid conversationId)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                var conversation = await conversationService.GetConversationByIdAsync(conversationId);
                if (conversation == null)
                {
                    await Clients.Caller.SendAsync("Error", "Conversation not found");
                    return;
                }

                // Check if user is participant
                var isParticipant = conversation.Participants.Any(p => p.UserId.ToString() == userId);

                // Nếu không phải thành viên, tự động thêm họ vào
                if (!isParticipant)
                {
                    // Auto-add user as participant
                    var userGuid = Guid.Parse(userId);
                    var participant = new ParticipantRequest
                    {
                        UserId = userGuid,
                        ConversationId = conversationId
                    };

                    await participantService.AddParticipantAsync(participant);
                    logger.LogInformation($"User {userId} auto-added to conversation {conversationId}");

                    // Cập nhật biến isParticipant sau khi thêm
                    isParticipant = true;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

                // Update user's conversation list
                if (UserConversations.ContainsKey(userId))
                {
                    UserConversations[userId].Add(conversationId.ToString());
                }

                await Clients.Caller.SendAsync("JoinedConversation", conversationId);
                logger.LogInformation($"User {userId} joined conversation {conversationId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error joining conversation {conversationId}");
                await Clients.Caller.SendAsync("Error", "Failed to join conversation");
            }
        }

        /// <summary>
        /// Send message to a conversation
        /// </summary>
        public async Task SendMessage(Guid conversationId, string content, Guid userId)
        {
            try
            {
                var currentUserId = Context.UserIdentifier;
                logger.LogInformation($"SendMessage: Token UserId={currentUserId}, Param UserId={userId}");
                //if (string.IsNullOrEmpty(currentUserId) || currentUserId != userId.ToString())
                //{
                //    await Clients.Caller.SendAsync("Error", "Unauthorized");
                //    return;
                //}

                if (string.IsNullOrWhiteSpace(content))
                {
                    await Clients.Caller.SendAsync("Error", "Message content cannot be empty");
                    return;
                }

                var conversation = await conversationService.GetConversationByIdAsync(conversationId);
                if (conversation == null)
                {
                    await Clients.Caller.SendAsync("Error", "Conversation not found");
                    return;
                }

                var isParticipant = conversation.Participants.Any(p => p.UserId == userId);
                if (!isParticipant)
                {
                    // Auto-add user as participant if they're not already
                    var participant = new ParticipantRequest { UserId = userId, ConversationId = conversationId };
                    await participantService.AddParticipantAsync(participant);
                    await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

                    // Update user's conversation list
                    if (UserConversations.ContainsKey(currentUserId))
                    {
                        UserConversations[currentUserId].Add(conversationId.ToString());
                    }

                    await Clients.Group(conversationId.ToString()).SendAsync("UserJoinedConversation", userId, currentUserId);
                }

                var message = new MessageRequest
                {
                    Content = content,
                    ConversationId = conversationId,
                    UserId = userId,
                };

                var messageResponse = await messageService.AddMessageAsync(message);

                // Send message to all participants in the conversation
                await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", messageResponse);

                logger.LogInformation($"Message sent from user {userId} to conversation {conversationId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error sending message to conversation {conversationId}");
                await Clients.Caller.SendAsync("Error", "Failed to send message");
            }
        }

        /// <summary>
        /// Start a new conversation with another user
        /// </summary>
        public async Task StartConversationWithUser(Guid targetUserId, string content)
        {
            try
            {
                var currentUserId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                var currentUserGuid = Guid.Parse(currentUserId);

                if (currentUserGuid == targetUserId)
                {
                    await Clients.Caller.SendAsync("Error", "Cannot start conversation with yourself");
                    return;
                }

                // Check if target user exists
                var targetUser = await userManager.FindByIdAsync(targetUserId.ToString());
                if (targetUser == null)
                {
                    await Clients.Caller.SendAsync("Error", "Target user not found");
                    return;
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    await Clients.Caller.SendAsync("Error", "Initial message content cannot be empty");
                    return;
                }

                // Create conversation with both users as participants
                var message = new MessageRequest { Content = content, UserId = currentUserGuid };
                var participants = new List<ParticipantRequest>
                {
                    new ParticipantRequest { UserId = currentUserGuid },
                    new ParticipantRequest { UserId = targetUserId }
                };

                var conversation = new ConversationRequest
                {
                    Messages = new List<MessageRequest> { message },
                    Participants = participants
                };

                var conversationResponse = await conversationService.AddConversationAsync(conversation);
                if (conversationResponse == null)
                {
                    await Clients.Caller.SendAsync("Error", "Failed to create conversation");
                    return;
                }

                // Add current user to conversation group
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationResponse.Id.ToString());

                // Add target user to conversation group if they're online
                if (UserConnections.TryGetValue(targetUserId.ToString(), out var targetConnectionId))
                {
                    await Groups.AddToGroupAsync(targetConnectionId, conversationResponse.Id.ToString());

                    // Update target user's conversation list
                    if (UserConversations.ContainsKey(targetUserId.ToString()))
                    {
                        UserConversations[targetUserId.ToString()].Add(conversationResponse.Id.ToString());
                    }
                }

                // Update current user's conversation list
                if (UserConversations.ContainsKey(currentUserId))
                {
                    UserConversations[currentUserId].Add(conversationResponse.Id.ToString());
                }

                // Notify both users about the new conversation
                await Clients.Group(conversationResponse.Id.ToString()).SendAsync("NewConversationCreated", conversationResponse);

                logger.LogInformation($"New conversation {conversationResponse.Id} created between users {currentUserGuid} and {targetUserId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting conversation");
                await Clients.Caller.SendAsync("Error", "Failed to start conversation");
            }
        }

        /// <summary>
        /// Get user's conversations (for reconnection scenarios)
        /// </summary>
        public async Task GetMyConversations()
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                var conversations = await GetUserConversationsAsync(Guid.Parse(userId));
                await Clients.Caller.SendAsync("MyConversations", conversations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user conversations");
                await Clients.Caller.SendAsync("Error", "Failed to get conversations");
            }
        }

        /// <summary>
        /// Typing indicator
        /// </summary>
        public async Task UserTyping(Guid conversationId, bool isTyping)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }

                await Clients.OthersInGroup(conversationId.ToString()).SendAsync("UserTyping", userId, isTyping);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in typing indicator for conversation {conversationId}");
            }
        }

        /// <summary>
        /// Mark messages as read
        /// </summary>
        public async Task MarkMessagesAsRead(Guid conversationId)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }

                // Here you can implement logic to mark messages as read in database
                await Clients.OthersInGroup(conversationId.ToString()).SendAsync("MessagesRead", userId, conversationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error marking messages as read for conversation {conversationId}");
            }
        }

        // Helper method to get user conversations
        private async Task<IEnumerable<dynamic>> GetUserConversationsAsync(Guid userId)
        {
            try
            {
                var userConversations = await conversationService.GetConversationByUserIdAsync(userId, 1, 100);
                return userConversations.Items;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting conversations for user {userId}");
                return new List<dynamic>();
            }
        }
    }
}
