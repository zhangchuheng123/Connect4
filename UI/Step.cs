using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI
{
    public class Step
    {
        bool User;
        public int x;
        public int y;

        public Step(bool isUser, int x, int y)
        {
            this.User = isUser;
            this.x = x;
            this.y = y;
        }

        public bool isUser()
        {
            return User;
        }

        public bool isA()
        {
            return User;
        }
    }
}
