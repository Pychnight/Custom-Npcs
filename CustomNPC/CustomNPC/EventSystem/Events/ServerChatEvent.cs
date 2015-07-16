using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CustomNPC.EventSystem.Events
{
    public class ServerChatEvent : HandledEventArgs
    {
        public MessageBuffer Buffer { get; set; }
        public string Text { get; set; }
        public int Who { get; set; }
    }
}
