﻿using System.Collections.Generic;

namespace _2Facies
{
    public interface IPacket
    {
        Dictionary<string, string> Tuple();
    }
    public interface IPublicData
    {
        string Id { get; set; }
    }
    public static class Packet
    {
        public static Dictionary<string, int> MaxLength = new Dictionary<string, int>()
        {
            {"id", 20 }, {"password", 25}, {"name", 20}, {"email", 320}
        };

        public enum ErrorCode
        {
            Connection = 101,
            WrongCode = 201,
            RoomJoin = 301,
            RoomLeave = 302,
            RoomFull = 303,
            RoomExist = 304,
            ChatSend = 401,
            ChatConnect = 402,
            ChatRecv = 403,
            RoomNotFound =  501,
            FormatError = 001,
            IncorrectTypeError = 002
        };

        public class Room : IPacket
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Participants { get; set; }
            public string Max { get; set; }
            public Room() { }
            public Room(string id) { Id = id; }
            public Room(string title, int participants, int max)
            {
                Title = title; Participants = participants.ToString();
                Max = max.ToString();
            }

            public Dictionary<string, string> Tuple()
            {
                var tuple = new Dictionary<string, string>()
                {
                    {"Title", Title } , {"Participants", Participants},
                };

                return tuple;
            }
        }

        public class Login : IPacket
        {
            public Login() { }
            public Login(string id, string password)
            {
                Id = id; Password = password;
            }

            public string Id { get; set; }
            public string Password { get; set; }

            public Dictionary<string, string> Tuple()
            {
                var tuple = new Dictionary<string, string>()
                {
                    {"id", Id } , {"password", Password},
                };

                return tuple;
            }
        }
        public class Register : IPacket
        {
            public Register() { }
            public Register(string id, string password, string name, string email, int age)
            {
                Id = id; Password = password;
                Name = name; Email = email; Age = age;
            }

            public string Id { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }

            public Dictionary<string, string> Tuple()
            {
                var tuple = new Dictionary<string, string>()
                {
                    {"id", Id } , {"password", Password},
                    {"name", Name }, {"email", Email}, {"age", Age.ToString()}
                };

                return tuple;
            }
            public void Bind(Dictionary<string, string> data)
            {
                Id = data["id"];
                Password = data["password"];
                Name = data["name"];
                Email = data["email"];
                Age = int.Parse(data["age"]);
            }
        }

        public class DataPublic : IPacket, IPublicData
        {
            public DataPublic() { }
            public DataPublic(Dictionary<string, string> jsonData)
            {
                Bind(jsonData);
            }
            //TESTING
            public DataPublic(string id, string name, string email, int age, bool logined)
            {
                Id = id;
                Name = name; Email = email;
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
            public bool Login { get; set; }

            public Dictionary<string, string> Tuple()
            {
                return new Dictionary<string, string>()
                {
                    {"userId", Id }, {"name", Name}, {"email", Email}, {"age", Age.ToString()}, {"login", (Login ? 1:0).ToString()}
                };
            }
            public void Bind(Dictionary<string, string> jsonData)
            {
                Id = jsonData["userId"];
                Name = jsonData["name"];
                Email = jsonData["email"];
                Age = int.Parse(jsonData["age"]);
                //Login = jsonData["login"];
            }
        }

    }
}
