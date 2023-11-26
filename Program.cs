using System;
using System.Collections.Generic;

public class ConsoleHelper
{
    public static void WriteLineColoredText(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    public static void WriteColoredText(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
    public static string ReadLineColoredText(ConsoleColor color)
    {
        Console.ForegroundColor = color;
        string x = Console.ReadLine();
        Console.ResetColor();
        return x;
    }

    public static string GetHiddenInput(ConsoleColor color)
    {
        var password = new System.Text.StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.ForegroundColor = color;
                Console.Write("*");
                Console.ResetColor();
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Length -= 1;
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        return password.ToString();
    }
}

public class Profile
{
    public string Name_ { get; set; }
    public string About_me { get; set; }

    public Profile(string Name, string Aboutme)
    {
        Name_ = Name;
        About_me = Aboutme;
    }

    public void PrintProfile()
    {
        Console.WriteLine();
        Console.Write("Name : ");
        ConsoleHelper.WriteLineColoredText(Name_, ConsoleColor.Yellow);
        Console.Write("About_me : ");
        ConsoleHelper.WriteLineColoredText(About_me, ConsoleColor.Yellow);
        Console.WriteLine();
    }
}

public class User
{
    public string username_ { get; set; }
    public string password_ { get; set; }
    public Profile profile_ { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
    public List<User> Following { get; set; } = new List<User>();
    public List<ChatMessage> ChatMessages_ { get; set; } = new List<ChatMessage>();
    public List<ChatMessage> NewChatMessages_ { get; set; } = new List<ChatMessage>();
    public int CurrentPostIndex { get; set; } = 0;

    public User(string username, string password, Profile profile)
    {
        username_ = username;
        password_ = password;
        profile_ = profile;
    }

    public void EditProfileName(string newName)
    {
        profile_.Name_ = newName;
    }

    public void EditProfileAboutMe(string newAboutMe)
    {
        profile_.About_me = newAboutMe;
    }

    public void ViewProfile(User userToView)
    {
        userToView.profile_.PrintProfile();
    }

    public void AddTextPost(User loggedInUser, string content, bool isPublic)
    {
        loggedInUser.Posts.Add(new TextPost(loggedInUser, content, isPublic));
    }

    public void AddImagePost(User loggedInUser, string imageUrl, bool isPublic)
    {
        loggedInUser.Posts.Add(new ImagePost(loggedInUser, imageUrl, isPublic));
    }

    public void AddVideoPost(User loggedInUser, string videoUrl, bool isPublic)
    {
        loggedInUser.Posts.Add(new VideoPost(loggedInUser, videoUrl, isPublic));
    }

    public void EditPost(Post post, string newContent, bool newIsPublic)
    {
        if (post.User_ == this)
        {
            post.IsPublic = newIsPublic;
            if (post is TextPost)
            {
                TextPost textPost = (TextPost)post;
                textPost.Text = newContent;
            }
            else if (post is ImagePost)
            {
                ImagePost imagePost = (ImagePost)post;
                imagePost.ImageUrl = newContent;
            }
            else if (post is VideoPost)
            {
                VideoPost videoPost = (VideoPost)post;
                videoPost.VideoUrl = newContent;
            }

            ConsoleHelper.WriteLineColoredText("\nPost edited successfully!\n", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelper.WriteLineColoredText("\nYou can only edit your own posts.\n", ConsoleColor.Red);
        }
    }

    public void Follow(User userToFollow)
    {
        if (userToFollow == this)
        {
            ConsoleHelper.WriteLineColoredText("\nYou can't follow yourself.\n", ConsoleColor.Red);
        }
        else
        {
            Following.Add(userToFollow);
            ConsoleHelper.WriteLineColoredText($"\nYou are now following {userToFollow}.\n", ConsoleColor.Green);
        }
    }

    public void Unfollow(User userToUnfollow)
    {
        if (userToUnfollow == this)
        {
            ConsoleHelper.WriteLineColoredText("\nYou can't unfollow yourself.\n", ConsoleColor.Red);
        }
        else
        {
            Following.Remove(userToUnfollow);
            ConsoleHelper.WriteLineColoredText($"\nYou have unfollowed {userToUnfollow}!\n", ConsoleColor.Green);
        }
    }

    public List<Post> GetTimelinePosts(List<User> userList)
    {
        List<Post> timelinePosts = new List<Post>();

        foreach (var user in Following)
        {
            foreach (var post in user.Posts)
            {
                if (post.IsPublic || post.whitelist.Contains(this))
                {
                    timelinePosts.Add(post);
                }
            }
        }
        foreach (var user in userList)
        {
            foreach (var post in user.Posts)
            {
                if (post.whitelist.Contains(this))
                {
                    timelinePosts.Add(post);
                }
            }
        }

        timelinePosts.Sort((p1, p2) => p2.Timestamp.CompareTo(p1.Timestamp));

        return timelinePosts;
    }

    public void DisplayPosts(List<User> userList)
    {
        CurrentPostIndex = 0;
        Posts.Sort((p1, p2) => p2.Timestamp.CompareTo(p1.Timestamp));

        bool continueDisplay = true;

        while (continueDisplay)
        {
            Console.Clear();
            Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
            Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");

            if (Posts.Count == 0)
            {
                ConsoleHelper.WriteLineColoredText("No posts to display.\n", ConsoleColor.Red);
                return;
            }

            Posts[CurrentPostIndex].Display();

            Console.WriteLine("----------------------------------------menu----------------------------------------");
            Console.WriteLine("1: Previous Post  2: View Comment  3: View whitelist  4: Edit Post  5: Next Post  6: Back to Main Menu");

            Console.Write("\nWhat do you have to do? : ");
            string option = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
            Console.WriteLine();
            switch (option)
            {
                case "1":
                    Console.Clear();
                    CurrentPostIndex = Math.Max(0, CurrentPostIndex - 1);
                    break;
                case "2":
                    foreach (var comment in Posts[CurrentPostIndex].Comments)
                    {
                        comment.Display();
                    }
                    Console.Write("Do you want to comment? (Y/N) : ");
                    string ifcomment = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    if (string.Equals(ifcomment, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("\nEnter your comment : ");
                        string commentText = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        AddComment(Posts[CurrentPostIndex], commentText);
                        ConsoleHelper.WriteLineColoredText("\nComment added successfully!\n", ConsoleColor.Green);
                    }
                    else { Console.Clear(); }
                    break;
                case "3":
                    Console.WriteLine("\nwhitelist : ");
                    if (Posts[CurrentPostIndex].whitelist.Count == 0)
                    {
                        ConsoleHelper.WriteLineColoredText("\nNo whitelist to display.", ConsoleColor.Red);
                    }
                    else
                    {
                        foreach (var user in Posts[CurrentPostIndex].whitelist)
                        {
                            ConsoleHelper.WriteLineColoredText("  " + user.username_, ConsoleColor.Yellow);
                        }
                    }
                    Console.Write("\nDo you want to add a whitelist?(Y/N) : ");
                    string ifwhitelist = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);

                    if (string.Equals(ifwhitelist, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("\nEnter User : ");
                        string username = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        User user = userList.Find(u => u.username_ == username);
                        if (user != null)
                        {
                            Posts[CurrentPostIndex].whitelist.Add(user);
                            ConsoleHelper.WriteLineColoredText("\nWhitelist added successfully!\nPlease wait.\n", ConsoleColor.Green);
                            Thread.Sleep(2000);
                        }
                        else { ConsoleHelper.WriteLineColoredText("\nUser not found.\nPlease wait.\n", ConsoleColor.Red);Thread.Sleep(2000); }
                    }
                    break;
                case "4":
                    Console.Clear();
                    Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                    Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
                    ConsoleHelper.WriteLineColoredText("your old post : \n", ConsoleColor.Yellow);
                    Posts[CurrentPostIndex].Display();
                    Console.WriteLine("\n▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄\n");
                    ConsoleHelper.WriteLineColoredText("your new post : \n", ConsoleColor.Yellow);
                    bool newIsPublic;
                    while (true)
                    {
                        Console.Write("Is this post public? (Y/N) : ");
                        string ifPublic = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        if (string.Equals(ifPublic, "Y", StringComparison.OrdinalIgnoreCase))
                        {
                            newIsPublic = true;
                            break;
                        }
                        else if (string.Equals(ifPublic, "N", StringComparison.OrdinalIgnoreCase))
                        {
                            newIsPublic = false;
                            break;
                        }
                        else
                        {
                            ConsoleHelper.WriteLineColoredText($"\n     Please enter Y or N only!", ConsoleColor.Yellow);
                            Console.WriteLine();
                        }
                    }

                    if (Posts[CurrentPostIndex] is TextPost)
                    {
                        Console.Write("new Text : ");
                        string newText = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        this.EditPost(Posts[CurrentPostIndex], newText, newIsPublic);
                    }
                    else if (Posts[CurrentPostIndex] is ImagePost)
                    {
                        Console.Write("new ImageUrl : ");
                        string newText = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        this.EditPost(Posts[CurrentPostIndex], newText, newIsPublic);
                    }
                    else if (Posts[CurrentPostIndex] is VideoPost)
                    {
                        Console.Write("new VideoUrl : ");
                        string newText = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        this.EditPost(Posts[CurrentPostIndex], newText, newIsPublic);
                    }
                    break;
                case "5":
                    Console.Clear();
                    CurrentPostIndex = Math.Min(Posts.Count - 1, CurrentPostIndex + 1);
                    break;
                case "6":
                    continueDisplay = false;
                    break;
                default:
                    ConsoleHelper.WriteLineColoredText("Invalid option. Please enter a number between 1 and 5.\n", ConsoleColor.Red);
                    break;
            }
        }
    }

    public void DisplayTimeline(List<User> userList)
    {
        CurrentPostIndex = 0;
        List<Post> timelinePosts = GetTimelinePosts(userList);

        bool continueDisplay = true;

        while (continueDisplay)
        {
            Console.Clear();
            Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
            Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");

            if (timelinePosts.Count == 0)
            {
                ConsoleHelper.WriteLineColoredText("No posts to display.\n", ConsoleColor.Red);
                return;
            }

            Post currentPost = timelinePosts[CurrentPostIndex];

            currentPost.Display();

            Console.WriteLine("---------------------------------menu---------------------------------");
            Console.WriteLine("1: Previous Post  2: View Comment  3: Next Post  4: Back to Main Menu");

            Console.Write("What do you have to do? :");
            string option = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
            switch (option)
            {
                case "1":
                    Console.Clear();
                    CurrentPostIndex = Math.Max(0, CurrentPostIndex - 1);
                    break;
                case "2":
                    foreach (var comment in currentPost.Comments)
                    {
                        comment.Display();
                    }
                    Console.Write("Do you want to comment? (Y/N) : ");
                    string ifcomment = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    if (string.Equals(ifcomment, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("\nEnter your comment : ");
                        string commentText = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                        AddComment(currentPost, commentText);
                        ConsoleHelper.WriteLineColoredText("\nComment added successfully!\n", ConsoleColor.Green);
                    }
                    else { Console.Clear(); }
                    break;
                case "3":
                    Console.Clear();
                    CurrentPostIndex = Math.Min(timelinePosts.Count - 1, CurrentPostIndex + 1);
                    break;
                case "4":
                    continueDisplay = false;
                    break;
                default:
                    ConsoleHelper.WriteLineColoredText("Invalid option. Please enter a number between 1 and 4.\n", ConsoleColor.Red);
                    break;
            }
        }
    }

    public void AddComment(Post post, string text)
    {
        post.Comments.Add(new Comment(this, text));
    }

    public void PostMenu(User loggedInUser)
    {
        bool postSuccess = true;

        while (postSuccess)
        {
            Console.WriteLine("------------------------------menu------------------------------");
            Console.WriteLine("1: Text Post  2: Image Post  3: Video Post  4: Back to Main Menu\n");
            Console.Write("What type of post do you want to create? : ");
            string postType = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
            Console.WriteLine();

            string ifPublic;

            switch (postType)
            {
                case "1":
                    Console.Write("Enter text for your post : ");
                    string textContent = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    Console.Write("Is this post public? (Y/N) : ");
                    ifPublic = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    if (string.Equals(ifPublic, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        loggedInUser.AddTextPost(loggedInUser, textContent, isPublic: true);
                        ConsoleHelper.WriteLineColoredText("Text post added successfully!\n", ConsoleColor.Green);
                    }
                    else if (string.Equals(ifPublic, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        loggedInUser.AddTextPost(loggedInUser, textContent, isPublic: false);
                        ConsoleHelper.WriteLineColoredText("Text post added successfully!\n", ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColoredText($"\n     Please enter Y or N only!", ConsoleColor.Yellow);
                        Console.WriteLine();
                    }
                    break;
                case "2":
                    Console.Write("Enter image URL for your post: ");
                    string imageUrl = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    Console.Write("Is this post public? (Y/N) : ");
                    ifPublic = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    if (string.Equals(ifPublic, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        loggedInUser.AddImagePost(loggedInUser, imageUrl, isPublic: true);
                        ConsoleHelper.WriteLineColoredText("Text post added successfully!\n", ConsoleColor.Green);
                    }
                    else if (string.Equals(ifPublic, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        loggedInUser.AddImagePost(loggedInUser, imageUrl, isPublic: false);
                        ConsoleHelper.WriteLineColoredText("Text post added successfully!\n", ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColoredText($"\n     Please enter Y or N only!", ConsoleColor.Yellow);
                        Console.WriteLine();
                    }
                    break;
                case "3":
                    Console.Write("Enter video URL for your post: ");
                    string videoUrl = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    Console.Write("Is this post public? (Y/N) : ");
                    ifPublic = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    if (string.Equals(ifPublic, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        loggedInUser.AddVideoPost(loggedInUser, videoUrl, isPublic: true);
                        ConsoleHelper.WriteLineColoredText("Text post added successfully!\n", ConsoleColor.Green);
                    }
                    else if (string.Equals(ifPublic, "N", StringComparison.OrdinalIgnoreCase))
                    {
                        loggedInUser.AddVideoPost(loggedInUser, videoUrl, isPublic: false);
                        ConsoleHelper.WriteLineColoredText("Text post added successfully!\n", ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColoredText($"\n     Please enter Y or N only!", ConsoleColor.Yellow);
                        Console.WriteLine();
                    }
                    break;
                case "4":
                    postSuccess = false;
                    break;
                default:
                    ConsoleHelper.WriteLineColoredText("Invalid option. Please enter a number between 1 and 4.\n", ConsoleColor.Red);
                    break;
            }
        }
    }

    public void SendChatMessage(User receiver, string message)
    {
        ChatMessages_.Add(new ChatMessage(this, receiver, message));
        receiver.ChatMessages_.Add(new ChatMessage(this, receiver, message));
        receiver.NewChatMessages_.Add(new ChatMessage(this, receiver, message));
    }

    public void DisplayMessage(User otherUser)
    {
        Console.WriteLine($"Chat history with {otherUser.profile_.Name_}:\n");

        foreach (var chatMessage in ChatMessages_
            .Where(m => (m.Receiver == otherUser && m.Sender == this) || (m.Sender == otherUser && m.Receiver == this))
            .OrderBy(m => m.Timetamp))
        {
            chatMessage.Display();
        }
        foreach (var chatMessage in NewChatMessages_
            .Where(m => (m.Receiver == otherUser && m.Sender == this) || (m.Sender == otherUser && m.Receiver == this))
            .OrderBy(m => m.Timetamp))
        {
            NewChatMessages_.Remove(chatMessage);
        }
        Console.WriteLine();
    }

    public void Chat_Message(List<User> userList)
    {
        Console.Write("Enter the username to chat with : ");
        string chatUsername = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
        Console.WriteLine();
        User chatUser = userList.Find(u => u.username_ == chatUsername);

        if (chatUser != null)
        {
            while (true)
            {
                this?.DisplayMessage(chatUser);

                Console.Write("Do you want to send a message? (Y/N) : ");
                string x = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);

                if (string.Equals(x, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("\nEnter your message : ");
                    string chatMessageText = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    this?.SendChatMessage(chatUser, chatMessageText);

                    ConsoleHelper.WriteLineColoredText("\nMessage sent successfully.\n", ConsoleColor.Green);
                }
                else if (string.Equals(x, "N", StringComparison.OrdinalIgnoreCase))
                { break; }
                else
                {
                    ConsoleHelper.WriteLineColoredText("\n     Please enter Y or N only!", ConsoleColor.Yellow);
                    Console.WriteLine();
                }
            }
            return;
        }
        else
        {
            ConsoleHelper.WriteLineColoredText("\nUser not found.\n", ConsoleColor.Red);
        }
    }
}

public class Post
{
    public User User_ { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<User> whitelist { get; set; } = new List<User>();
    public bool IsPublic { get; set; }

    public Post(User user, string content, bool isPublic)
    {
        User_ = user;
        Content = content;
        Timestamp = DateTime.Now;
        IsPublic = isPublic;
    }

    public virtual void Display()
    {
        Console.Write("Post by : ");
        ConsoleHelper.WriteLineColoredText(User_.profile_.Name_, ConsoleColor.Yellow);
        Console.Write("Content : ");
        ConsoleHelper.WriteLineColoredText(Content, ConsoleColor.Yellow);
        Console.Write("Privacy : ");
        ConsoleHelper.WriteLineColoredText(IsPublic ? "Public" : "Private", ConsoleColor.Yellow);
        Console.WriteLine("----------------------------------------------------------");
    }
}

public class TextPost : Post
{
    public string Text { get; set; }
    public TextPost(User user, string content, bool isPublic) : base(user, "Text Post", isPublic)
    {
        Text = content;
    }
    public override void Display()
    {
        base.Display();
        Console.Write("Text : ");
        ConsoleHelper.WriteLineColoredText(Text, ConsoleColor.Yellow);
        Console.WriteLine("----------------------------------------------------------");
        Console.Write("Timestamp: ");
        ConsoleHelper.WriteLineColoredText("" + Timestamp, ConsoleColor.Yellow);
        Console.Write("Number of Comments : ");
        ConsoleHelper.WriteLineColoredText(Comments.Count.ToString(), ConsoleColor.Yellow);
    }
}

public class ImagePost : Post
{
    public string ImageUrl { get; set; }

    public ImagePost(User user, string imageUrl, bool isPublic) : base(user, "Image Post", isPublic)
    {
        ImageUrl = imageUrl;
    }

    public override void Display()
    {
        base.Display();
        Console.Write("Image URL: ");
        ConsoleHelper.WriteLineColoredText(ImageUrl, ConsoleColor.Yellow);
        Console.WriteLine("----------------------------------------------------------");
        Console.Write("Timestamp: ");
        ConsoleHelper.WriteLineColoredText("" + Timestamp, ConsoleColor.Yellow);
        Console.Write("Number of Comments : ");
        ConsoleHelper.WriteLineColoredText(Comments.Count.ToString(), ConsoleColor.Yellow);
    }
}

public class VideoPost : Post
{
    public string VideoUrl { get; set; }

    public VideoPost(User user, string videoUrl, bool isPublic) : base(user, "Video Post", isPublic)
    {
        VideoUrl = videoUrl;
    }

    public override void Display()
    {
        base.Display();
        Console.Write("Video URL: ");
        ConsoleHelper.WriteLineColoredText(VideoUrl, ConsoleColor.Yellow);
        Console.WriteLine("----------------------------------------------------------");
        Console.Write("Timestamp: ");
        ConsoleHelper.WriteLineColoredText("" + Timestamp, ConsoleColor.Yellow);
        Console.Write("Number of Comments : ");
        ConsoleHelper.WriteLineColoredText(Comments.Count.ToString(), ConsoleColor.Yellow);
    }
}

public class Comment
{
    public User User_ { get; set; }
    public string Text { get; set; }

    public Comment(User user, string text)
    {
        User_ = user;
        Text = text;
    }

    public void Display()
    {
        Console.Write($"Comment by ");
        ConsoleHelper.WriteColoredText(User_.profile_.Name_, ConsoleColor.Yellow);
        Console.Write(" : ");
        ConsoleHelper.WriteLineColoredText(Text, ConsoleColor.Yellow);
    }
}

public class ChatMessage
{
    public User Sender { get; set; }
    public User Receiver { get; set; }
    public string Message { get; set; }
    public DateTime Timetamp { get; set; }

    public ChatMessage(User sender, User receiver, string message)
    {
        Sender = sender;
        Receiver = receiver;
        Message = message;
        Timetamp = DateTime.Now;
    }

    public void Display()
    {
        Console.Write("From : ");
        ConsoleHelper.WriteColoredText(Sender.profile_.Name_, ConsoleColor.Yellow);
        Console.Write("| To : ");
        ConsoleHelper.WriteLineColoredText(Receiver.profile_.Name_, ConsoleColor.Yellow);
        Console.Write("Message : ");
        ConsoleHelper.WriteLineColoredText(Message, ConsoleColor.Yellow);
        Console.Write("Sent at : ");
        ConsoleHelper.WriteLineColoredText("" + Timetamp, ConsoleColor.Yellow);
        Console.WriteLine("-----------------------------------------------------\n");
    }
}

public class Ui
{
    public string Login(List<User> userList)
    {
        string user_login = "";
        bool success = true;

        while (success)
        {
            Logo(ConsoleColor.White);
            Console.Write("                       You have an account?(Y/N) : ");
            string menu = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
            Console.WriteLine();

            if (string.Equals(menu, "Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
                Console.WriteLine("                                   Log in\n");
                Console.Write("                          username : ");
                string username = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                Console.Write("                          password : ");
                string password = ConsoleHelper.GetHiddenInput(ConsoleColor.Yellow);
                Console.WriteLine("\n");

                User loggedInUser = userList.Find(u => u.username_ == username && u.password_ == password);

                if (loggedInUser != null)
                {
                    user_login = loggedInUser.username_;

                    ConsoleHelper.WriteLineColoredText("                              Login successful!\n", ConsoleColor.Green);

                    Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                    Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");

                    Console.Write("                          user_login : ");
                    ConsoleHelper.WriteLineColoredText(user_login, ConsoleColor.Yellow);

                    success = false;
                }
                else
                {
                    ConsoleHelper.WriteColoredText("The username or password is incorrect. Please try again.\n", ConsoleColor.Red);
                }
            }
            else if (string.Equals(menu, "N", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
                Console.WriteLine("                                  Register\n");
                Console.Write("                          username : ");
                string username = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                Console.Write("                          password : ");
                string password = ConsoleHelper.GetHiddenInput(ConsoleColor.Yellow);
                Console.WriteLine("\n");

                User loggedInUser = userList.Find(u => u.username_ == username);

                Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
                Console.WriteLine("                               Create Profile\n");

                if (loggedInUser == null)
                {
                    user_login = username;

                    Console.Write("                      displayname : ");
                    string Name = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    Console.Write("                          Aboutme : ");
                    string Aboutme = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);

                    Profile profile = new Profile(Name, Aboutme);

                    userList.Add(new User(username, password, profile));

                    ConsoleHelper.WriteLineColoredText("\n                              Register successful!\n", ConsoleColor.Green);
                    Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                    Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");

                    Console.Write("                          user_login : ");
                    ConsoleHelper.WriteLineColoredText(user_login, ConsoleColor.Yellow);

                    success = false;
                }
                else
                {
                    ConsoleHelper.WriteLineColoredText("This username already exists Please try again.\n", ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleHelper.WriteLineColoredText("                          Please enter Y or N only!\n", ConsoleColor.Yellow);
            }
        }
        Console.WriteLine();
        return user_login;
    }

    public static void Logo(ConsoleColor color)
    {
        Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
        Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
        ConsoleHelper.WriteLineColoredText("                      ████████╗░█████╗░░█████╗░░█████╗░\n" +
                                           "                      ╚══██╔══╝██╔══██╗██╔══██╗██╔══██╗\n" +
                                           "                      ░░░██║░░░██║░░██║███████║██║░░██║\n" +
                                           "                      ░░░██║░░░██║░░██║██╔══██║██║░░██║\n" +
                                           "                      ░░░██║░░░╚█████╔╝██║░░██║╚█████╔╝\n" +
                                           "                      ░░░╚═╝░░░░╚════╝░╚═╝░░╚═╝░╚════╝░\n", color);
        Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
        Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Ui ui = new Ui();
        Console.Clear();
        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :--------------------: 0%", ConsoleColor.Blue);
        Thread.Sleep(100);
        Console.Clear();
        Ui.Logo(ConsoleColor.Blue);
        Profile profileSarto = new Profile("Sarto", "...");
        Profile profileAof = new Profile("Aof", "...");
        ConsoleHelper.WriteLineColoredText("                     Loading :██------------------: 10%", ConsoleColor.Blue);
        Thread.Sleep(100);
        Console.Clear();

        List<User> userList = new List<User>{
            new User("Sarto","1023",profileSarto),
            new User("Aof","1055",profileAof),
        };

        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :████----------------: 20%", ConsoleColor.Blue);
        Thread.Sleep(100);
        Console.Clear();

        User User_Test_Sarto = userList.Find(u => u.username_ == "Sarto");
        User User_Test_Aof = userList.Find(u => u.username_ == "Aof");
        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :██████--------------: 30%", ConsoleColor.Blue);
        Thread.Sleep(100);

        User_Test_Sarto?.AddTextPost(User_Test_Sarto, "This is my first text post!", isPublic: true);
        User_Test_Aof?.AddTextPost(User_Test_Aof, "This is my first text post!", isPublic: true);
        Thread.Sleep(500);
        Console.Clear();
        User_Test_Aof?.AddTextPost(User_Test_Aof, "This is my private post!", isPublic: false);
        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :██████████----------: 50%", ConsoleColor.Blue);
        Thread.Sleep(500);
        User_Test_Sarto?.AddImagePost(User_Test_Sarto, "https://i.pinimg.com/564x/53/7e/31/537e315ad64391e28765ef86ba555e67.jpg", isPublic: true);
        Console.Clear();
        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :████████████--------: 60%", ConsoleColor.Blue);
        Thread.Sleep(500);
        User_Test_Sarto?.AddVideoPost(User_Test_Sarto, "https://youtu.be/dQw4w9WgXcQ", isPublic: true);
        Console.Clear();
        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :██████████████------: 70%", ConsoleColor.Blue);
        Thread.Sleep(500);
        User_Test_Aof?.AddTextPost(User_Test_Aof, "Another post!", isPublic: true);
        Console.Clear();
        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :████████████████----: 80%", ConsoleColor.Blue);
        Thread.Sleep(100);
        Console.Clear();

        Ui.Logo(ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                     Loading :████████████████████: 100%", ConsoleColor.Blue);
        ConsoleHelper.WriteLineColoredText("                               Successfully loaded", ConsoleColor.Green);
        Thread.Sleep(1000);
        Console.Clear();
        string user_login = ui.Login(userList);

        User loggedInUser = userList.Find(u => u.username_ == user_login);

        ConsoleHelper.WriteLineColoredText("\nLoading...", ConsoleColor.Blue);
        Thread.Sleep(1000);
        Console.Clear();

        while (true)
        {
            loggedInUser = userList.Find(u => u.username_ == user_login);

            Ui.Logo(ConsoleColor.White);
            Console.Write("                          user login : ");
            ConsoleHelper.WriteLineColoredText(user_login, ConsoleColor.Yellow);
            Console.Write("                        New Messages : ");
            ConsoleHelper.WriteLineColoredText(loggedInUser.NewChatMessages_.Count.ToString() + "!", ConsoleColor.Red);
            Console.WriteLine("\n----------------------------------------menu----------------------------------------");
            Console.WriteLine("1: View feed  2: View my post  3: Create post  4: View New Messages  5: Chat Message\n" +
                              "6: Follow  7: Unfollow  8: View Following  9: View Profile  10: Edit Profile\n" +
                              "11: Logout  12: End\n");
            ConsoleHelper.WriteLineColoredText("                        Please enter numbers only.\n", ConsoleColor.Yellow);
            Console.Write("                      What do you have to do? : ");
            string menu = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
            Console.Clear();

            Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
            Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");

            switch (menu)
            {
                case "1":
                    loggedInUser?.DisplayTimeline(userList);
                    break;
                case "2":
                    loggedInUser?.DisplayPosts(userList);
                    break;
                case "3":
                    loggedInUser?.PostMenu(loggedInUser);
                    break;
                case "4":
                    Console.WriteLine("---------New Messages---------");
                    List<User> chatMessageList = new List<User> { };
                    foreach (var chatMessage in loggedInUser.NewChatMessages_)
                    {
                        User MessageInUser = chatMessageList.Find(u => u.username_ == chatMessage.Sender.username_);

                        if (MessageInUser == null)
                        {
                            chatMessageList.Add(chatMessage.Sender);
                            int X = 0;
                            foreach (var UserchatMessage in loggedInUser.NewChatMessages_)
                            {
                                if (UserchatMessage.Sender == chatMessage.Sender)
                                {
                                    X++;
                                }
                            }
                            Console.Write(" From : ");
                            ConsoleHelper.WriteColoredText(chatMessage.Sender.username_, ConsoleColor.Yellow);
                            Console.Write(" : ");
                            ConsoleHelper.WriteLineColoredText("" + X + " New Messages!", ConsoleColor.Red);
                        }
                    }

                    Console.Write("\nDo you want to send a message? (Y/N) : ");
                    string ifwant = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    if (string.Equals(ifwant, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\n▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                        Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
                        loggedInUser?.Chat_Message(userList);
                    }
                    break;
                case "5":
                    loggedInUser?.Chat_Message(userList);
                    break;
                case "6":
                    Console.Write("Enter the username to follow: ");
                    string usernameToFollow = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    User userToFollow = userList.Find(u => u.username_ == usernameToFollow);

                    if (userToFollow != null)
                    {
                        loggedInUser?.Follow(userToFollow);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColoredText("\nUser not found.\n", ConsoleColor.Red);
                    }
                    break;
                case "7":
                    Console.Write("Enter the username to unfollow: ");
                    string usernameToUnfollow = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    User userToUnfollow = loggedInUser.Following.Find(u => u.username_ == usernameToUnfollow || u.username_ == loggedInUser.username_);

                    if (userToUnfollow != null)
                    {
                        loggedInUser?.Unfollow(userToUnfollow);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColoredText("\nUser not found in your following list.\n", ConsoleColor.Red);
                    }
                    break;
                case "8":
                    Console.WriteLine("Following list : \n");
                    foreach (var followedUser in loggedInUser.Following)
                    {
                        ConsoleHelper.WriteLineColoredText("  " + followedUser.username_, ConsoleColor.Yellow);
                    }
                    Console.WriteLine();
                    break;
                case "9":
                    Console.Write("Enter the username to view profile : ");
                    string usernameToView = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    User userToView = userList.Find(u => u.username_ == usernameToView);

                    if (userToView != null)
                    {
                        loggedInUser?.ViewProfile(userToView);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColoredText("\nUser not found.\n", ConsoleColor.Red);
                    }
                    break;
                case "10":
                    loggedInUser?.ViewProfile(loggedInUser);
                    Console.WriteLine("----------menu----------");
                    Console.WriteLine("  1: Edit name\n  2: Edit aboutme\n  3: Back to Main Menu\n");
                    Console.Write("What do you want to Edit? : ");
                    string Edit_menuprofile = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);
                    switch (Edit_menuprofile)
                    {
                        case "1":
                            Console.Write("Enter your new name : ");
                            string new_name = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);

                            loggedInUser?.EditProfileName(new_name);
                            ConsoleHelper.WriteLineColoredText("\nEdit successful!", ConsoleColor.Green);
                            break;
                        case "2":
                            Console.Write("Enter your new aboutme : ");
                            string new_aboutme = ConsoleHelper.ReadLineColoredText(ConsoleColor.Yellow);

                            loggedInUser?.EditProfileAboutMe(new_aboutme);
                            ConsoleHelper.WriteLineColoredText("\nEdit successful!", ConsoleColor.Green);
                            break;
                        default:
                            break;
                    }
                    break;
                case "11":
                    ConsoleHelper.WriteLineColoredText("\n       logout successful!\n", ConsoleColor.Green);
                    user_login = ui.Login(userList);
                    break;
                default:
                    ConsoleHelper.WriteLineColoredText("                         ███████╗███╗░░██╗██████╗░\n" +
                                                       "                         ██╔════╝████╗░██║██╔══██╗\n" +
                                                       "                         █████╗░░██╔██╗██║██║░░██║\n" +
                                                       "                         ██╔══╝░░██║╚████║██║░░██║\n" +
                                                       "                         ███████╗██║░╚███║██████╔╝\n" +
                                                       "                         ╚══════╝╚═╝░░╚══╝╚═════╝░\n", ConsoleColor.Red);
                    Console.WriteLine("▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄ ▄▄");
                    Console.WriteLine("░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░ ░░\n");
                    return;
            }
        }
    }
}
