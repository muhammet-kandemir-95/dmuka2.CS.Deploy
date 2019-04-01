using System;
using System.Collections.Generic;
using System.Text;

namespace dmuka2.CS.Deploy
{
    public class Command
    {
        #region Variables
        /// <summary>
        /// User command name.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// This is for help.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// What will it do?
        /// </summary>
        public Action Action { get; private set; }
        #endregion

        #region Constructors
        public Command(string name, string description, Action action)
        {
            this.Name = name;
            this.Description = description;
            this.Action = action;
        }
        #endregion
    }
}
