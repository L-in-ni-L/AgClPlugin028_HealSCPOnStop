using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgClPlugin028_HealSCPOnStop
{
    public class Config : IConfig
    {
        [Description("是否启用scp站立回血")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;
        [Description("站立回血所需静止时间")]
        public int HealDelay { get; set; } = 20;

        [Description("每秒恢复血量")]

        public float HPPerTime { get; set; } = 3;
    }
}
