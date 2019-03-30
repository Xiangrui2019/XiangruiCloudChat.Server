﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Services
{
    public class Counter
    {
        private readonly object _obj = new object();
        public int _Counted = 0;
        public int GetUniqueNo
        {
            get
            {
                lock (_obj)
                {
                    _Counted++;
                }
                return _Counted;
            }
        }
    }
}
