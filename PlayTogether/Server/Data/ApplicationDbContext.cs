using PlayTogether.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayTogether.Shared.Models;

namespace PlayTogether.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<ApplicationUserDetails> ApplicationUserDetails { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Gender> Genders { get; set; }

        public DbSet<GameSkillLevel> GameSkillLevels { get; set; }

        public DbSet<GamingPlatform> GamingPlatforms { get; set; }

        public DbSet<AppSetting> AppSettings { get; set; }

        public DbSet<ApplicationUser_GamingPlatform> ApplicationUser_GamingPlatform { get; set; }

        public DbSet<GameGenre> GameGenres { get; set; }

        public DbSet<ApplicationUser_GameGenre> ApplicationUser_GameGenres { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameGenre_Game> GameGenre_Games { get; set; }

        public DbSet<GamingPlatform_Game> GamingPlatform_Games { get; set; }

        public DbSet<ApplicationUser_Game> ApplicationUser_Games { get; set; }

        public DbSet<FriendRequestStatusType> FriendRequestStatusTypes { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        public DbSet<ApplicationUser_Friend> ApplicationUser_Friends { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<MessageConnection> MessageConnections { get; set; }

        public DbSet<ApplicationUser_MessageConnection> ApplicationUser_MessageConnections { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<ApplicationUser_Conversation> ApplicationUser_Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasKey(detail => detail.ApplicationUserId)
                .HasName("PrimaryKey_ApplicationUserId");

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasOne(detail => detail.ApplicationUser)
                .WithOne(user => user.ApplicationUserDetails)
                .IsRequired()
                .HasForeignKey<ApplicationUserDetails>(detail => detail.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_UserDetails");

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasOne(detail => detail.CountryOfResidence)
                .WithOne()
                .IsRequired()
                .HasForeignKey<ApplicationUserDetails>(detail => detail.CountryOfResidenceId)
                .HasConstraintName("ForeignKey_User_Country")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasOne(detail => detail.Gender)
                .WithOne()
                .IsRequired()
                .HasForeignKey<ApplicationUserDetails>(detail => detail.GenderId)
                .HasConstraintName("ForeignKey_User_Gender")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUser_GamingPlatform>()
                .HasKey(mapping => new { mapping.ApplicationUserId, mapping.GamingPlatformId })
                .HasName("PrimaryKey_ApplicationUserId_GamingPlatformId");

            modelBuilder.Entity<ApplicationUser_GamingPlatform>()
                .HasOne(mapping => mapping.ApplicationUser)
                .WithMany(user => user.GamingPlatforms)
                .HasForeignKey(mapping => mapping.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_GamingPlatform_ApplicationUserId");

            modelBuilder.Entity<ApplicationUser_GamingPlatform>()
                .HasOne(mapping => mapping.GamingPlatform)
                .WithMany(platform => platform.Users)
                .HasForeignKey(mapping => mapping.GamingPlatformId)
                .HasConstraintName("ForeignKey_User_GamingPlatform_GamingPlatformId");

            modelBuilder.Entity<ApplicationUser_GameGenre>()
                .HasKey(mapping => new { mapping.ApplicationUserId, mapping.GameGenreId })
                .HasName("PrimaryKey_ApplicationUserId_GameGenreId");

            modelBuilder.Entity<ApplicationUser_GameGenre>()
                .HasOne(mapping => mapping.ApplicationUser)
                .WithMany(user => user.GameGenres)
                .HasForeignKey(mapping => mapping.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_GameGenre_ApplicationUserId");

            modelBuilder.Entity<ApplicationUser_GameGenre>()
                .HasOne(mapping => mapping.GameGenre)
                .WithMany(platform => platform.Users)
                .HasForeignKey(mapping => mapping.GameGenreId)
                .HasConstraintName("ForeignKey_User_GamingPlatform_GameGenreId");

            modelBuilder.Entity<ApplicationUser_Game>()
                .HasKey(mapping => new { mapping.ApplicationUserId, mapping.GameId })
                .HasName("PrimaryKey_ApplicationUserId_GameId");

            modelBuilder.Entity<ApplicationUser_Game>()
                .HasOne(mapping => mapping.ApplicationUser)
                .WithMany(user => user.Games)
                .HasForeignKey(mapping => mapping.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_Game_ApplicationUserId");

            modelBuilder.Entity<ApplicationUser_Game>()
                .HasOne(mapping => mapping.Game)
                .WithMany(game => game.Users)
                .HasForeignKey(mapping => mapping.GameId)
                .HasConstraintName("ForeignKey_User_Game_GameId");

            modelBuilder.Entity<GamingPlatform_Game>()
                .HasKey(mapping => new { mapping.GamingPlatformId, mapping.GameId })
                .HasName("PrimaryKey_GamingPlatformId_GameId");

            modelBuilder.Entity<GamingPlatform_Game>()
                .HasOne(mapping => mapping.GamingPlatform)
                .WithMany(platform => platform.Games)
                .HasForeignKey(mapping => mapping.GamingPlatformId)
                .HasConstraintName("ForeignKey_GamingPlatform_Game_GamingPlatformId");

            modelBuilder.Entity<GamingPlatform_Game>()
                .HasOne(mapping => mapping.Game)
                .WithMany(game => game.GamingPlatforms)
                .HasForeignKey(mapping => mapping.GameId)
                .HasConstraintName("ForeignKey_GamingPlatform_Game_GameId");

            modelBuilder.Entity<GameGenre_Game>()
                .HasKey(mapping => new { mapping.GameGenreId, mapping.GameId })
                .HasName("PrimaryKey_GameGenreId_GameId");

            modelBuilder.Entity<GameGenre_Game>()
                .HasOne(mapping => mapping.GameGenre)
                .WithMany(platform => platform.Games)
                .HasForeignKey(mapping => mapping.GameGenreId)
                .HasConstraintName("ForeignKey_GameGenre_Game_GameGenreId");

            modelBuilder.Entity<GameGenre_Game>()
                .HasOne(mapping => mapping.Game)
                .WithMany(game => game.GameGenres)
                .HasForeignKey(mapping => mapping.GameId)
                .HasConstraintName("ForeignKey_GameGenre_Game_GameId");

            modelBuilder.Entity<ApplicationUser_Game>()
                .HasOne(mapping => mapping.GameSkillLevel)
                .WithMany(level => level.UserGames)
                .HasForeignKey(mapping => mapping.GameSkillLevelId)
                .HasConstraintName("ForeignKey_User_Game_GameSkillLevelId");

            modelBuilder.Entity<FriendRequest>()
                .HasOne(request => request.FromUser)
                .WithMany(user => user.SentFriendRequests)
                .IsRequired()
                .HasForeignKey(request => request.FromUserId)
                .HasConstraintName("ForeignKey_FriendRequest_User_FromUserId")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(request => request.ToUser)
                .WithMany(user => user.ReceivedFriendRequests)
                .IsRequired()
                .HasForeignKey(request => request.ToUserId)
                .HasConstraintName("ForeignKey_FriendRequest_User_ToUserId")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendRequest>()
                .HasOne<FriendRequestStatusType>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(request => request.FriendRequestStatusId)
                .HasConstraintName("ForeignKey_FriendRequest_FriendRequestStatusId")
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<ApplicationUser_Friend>()
                .HasOne(mapping => mapping.ApplicationUser)
                .WithMany(user => user.Friends)
                .HasForeignKey(mapping => mapping.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_Friend_ApplicationUserId")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUser_Friend>()
                .HasOne(mapping => mapping.FriendUser)
                .WithMany()
                .HasForeignKey(mapping => mapping.FriendUserId)
                .HasConstraintName("ForeignKey_User_Friend_FriendUserId")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(request => request.FromUser)
                .WithMany(user => user.SentMessages)
                .IsRequired()
                .HasForeignKey(request => request.FromUserId)
                .HasConstraintName("ForeignKey_Message_User_FromUserId")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.Conversation)
                .WithMany(conversation => conversation.Messages)
                .IsRequired()
                .HasForeignKey(message => message.ConversationId)
                .HasConstraintName("ForeignKey_Message_Conversation_ConversationId")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUser_Conversation>()
                .HasKey(mapping => new { mapping.ApplicationUserId, mapping.ConversationId })
                .HasName("PrimaryKey_ApplicationUserId_ConversationId");

            modelBuilder.Entity<ApplicationUser_Conversation>()
                .HasOne(mapping => mapping.ApplicationUser)
                .WithMany(user => user.Conversations)
                .HasForeignKey(mapping => mapping.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_Conversation_ApplicationUserId");

            modelBuilder.Entity<ApplicationUser_Conversation>()
                .HasOne(mapping => mapping.Conversation)
                .WithMany(conversation => conversation.Users)
                .HasForeignKey(mapping => mapping.ConversationId)
                .HasConstraintName("ForeignKey_User_Conversation_ConversationId");

            modelBuilder.Entity<ApplicationUser_MessageConnection>()
                .HasKey(mapping => new { mapping.ApplicationUserId, mapping.MessageConnectionId })
                .HasName("PrimaryKey_ApplicationUserId_MessageConnectionId");

            modelBuilder.Entity<ApplicationUser_MessageConnection>()
                .HasOne(mapping => mapping.ApplicationUser)
                .WithMany(user => user.MessageConnections)
                .HasForeignKey(mapping => mapping.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_MessageConnection_ApplicationUserId");

            modelBuilder.Entity<ApplicationUser_MessageConnection>()
                .HasOne(mapping => mapping.MessageConnection)
                .WithMany(conversation => conversation.Users)
                .HasForeignKey(mapping => mapping.MessageConnectionId)
                .HasConstraintName("ForeignKey_User_MessageConnection_MessageConnectionId");

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedConversations)
                .IsRequired(false)
                .HasForeignKey(c => c.CreatedByUserId)
                .HasConstraintName("ForeignKey_Conversation_User_CreatedByUserId");
        }
    }
}