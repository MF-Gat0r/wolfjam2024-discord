using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using System.IO;
using System.Globalization;
/// <summary>
/// Name: Twitch Connect
/// Purpose: Connect to specfied streamer's twitch chat and read chatlogs to parge commands
/// Author(s): Katie Hellmann, Gator Flack PabloMakes Twitch Integration Tutorial
/// </summary>
public class TwitchConnect : MonoBehaviour
{
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;
    private float pingCounter = 0f;

    public InputField usernameInput;
    public InputField channelInput;
    public InputField oauthInput;
    public Button connectButton;

    private string username;
    private string channelName;
    private string oauthToken;

    public event Action<string, string> OnChatMessage;
    void Start()
    {
        connectButton.onClick.AddListener(InitializeTwitchConnection);
    }

    void InitializeTwitchConnection()
    {
        username = usernameInput.text.Trim();
        channelName = channelInput.text.Trim();
        oauthToken = oauthInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(channelName) || string.IsNullOrEmpty(oauthToken))
        {
            Debug.LogError("Please enter valid Twitch credentials.");
            return;
        }

        ConnectToTwitch();
    }

    void ConnectToTwitch()
    {
        try
        {
            twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
            reader = new StreamReader(twitchClient.GetStream());
            writer = new StreamWriter(twitchClient.GetStream()) { AutoFlush = true };

            // Authenticate using inputted credentials
            writer.WriteLine($"PASS {oauthToken}");
            writer.WriteLine($"NICK {username}");
            writer.WriteLine($"JOIN #{channelName}");

            Debug.Log("Connected to Twitch Chat!");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to Twitch: " + e.Message);
        }
    }

    void Update()
    {
        if (twitchClient == null || !twitchClient.Connected)
        {
            return;
        }
        pingCounter += Time.deltaTime;
        if (pingCounter > 60) // Send PING every 60 seconds ( Can be changed )
        {
            writer.WriteLine("PING :tmi.twitch.tv");
            writer.Flush();
            pingCounter = 0;
        }

        ReadChat();
    }
    private void Awake()
    {
        ConnectToTwitch();
    }
    // Update is called once per frame
    void Update()
    {
        pingCounter += Time.deltaTime; //count time
        //constantly reconnect
        if (pingCounter > 60)
        {
            Writer.WriteLine("PING " + URL);
            Writer.Flush();
            pingCounter = 0;
        }
        //if not connected, connect to twitch
        if (!TwitchTV.Connected)
        {
            ConnectToTwitch();
        }
        //if available parse commands
        if (TwitchTV.Available > 0)
        {
            string message = Reader.ReadLine();
            if (message.Contains("PRIVMSG"))
            {
                int splitPoint = message.IndexOf("!");
                string chatter = message.Substring(1, splitPoint - 1);
                splitPoint = message.IndexOf(":", 1);
                string msg = message.Substring(splitPoint + 1);

                OnChatMessage?.Invoke(chatter, msg);

            }
            //print to unity console for debugging purposes
            print(message);
        }

    }
    public class TwitchChat : MonoBehaviour
    {
        private TcpClient twitchClient;
        private StreamReader reader;
        private StreamWriter writer;

        public InputField usernameInput;
        public InputField channelInput;
        public InputField oauthInput;
        public Button connectButton;

        private string username;
        private string channelName;
        private string oauthToken;
    }
}

