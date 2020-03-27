using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BlueQuery.Commands.Crafting.CommandStorageTypes
{
    /// <summary>
    ///     Saves data related to the last time ?craft was called;
    /// </summary>
    public class SavedCraftInstructions
    {
        private const int TIMEOUT_TIME = 20000;
        private static readonly Timer Timer = new Timer(TIMEOUT_TIME);

        private static SCIContent content;
        /// <summary>
        ///     Wrapper that contains the Keys and Quantity properties needed for crafting.
        /// </summary>
        public static SCIContent Content {
            get => content; 
            set
            {
                content = value;
                Timer.Start();
            }
        }

        /// <summary>
        ///     Add the Timer.Elasped handler so we can start countdowns when the content changes.
        /// </summary>
        public SavedCraftInstructions()
        {
            Timer.Elapsed += TimerElasped_WipeContent;
        }        
        /// <summary>
        ///     Unbind the Timer.ElaspedHandler when being disposed so our Timer doesn't have a dangling method pointer.
        /// </summary>
        ~SavedCraftInstructions()
        {
            Timer.Elapsed -= TimerElasped_WipeContent;
        }

        /// <summary>
        ///     Triggers when the timer has elasped.
        ///     Wipes content of this class.
        /// </summary>
        private void TimerElasped_WipeContent(object sender, ElapsedEventArgs e)
        {
            Content = null;
            Timer.Stop();
        }

        /// <summary>
        ///     Contains the content required for choosing an item to craft and passing the information along to the next steps.
        /// </summary>
        public class SCIContent
        {
            public string[] Keys { get; private set; }
            public int Amount { get; private set; }

            public SCIContent(string[] _keys, int _amount)
            {
                Keys = _keys;
                Amount = _amount;
            }
        }
    }
}
