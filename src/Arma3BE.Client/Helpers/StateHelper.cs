using System;
using System.Collections.Generic;
using System.Linq;
using Arma3BEClient.Updater.Models;

namespace Arma3BEClient.Helpers
{
    public class StateHelper<T> where T : class
    {
        private ServerMessage _previousMessage;
        private List<T> _previousRequest;

        protected bool HaveChanges(ServerMessage newMessage)
        {
            var temp = _previousMessage;
            _previousMessage = newMessage;

            if (temp != null)
            {
                return string.CompareOrdinal(temp.Message, newMessage.Message) != 0;
            }

            return true;
        }

        protected bool HaveChanges<TK>(List<T> newList, Func<T, TK> comparer)
        {
            var temp = _previousRequest;
            _previousRequest = newList;

            if (temp != null && temp.Count == newList.Count)
            {
                var count = temp.Count;
                var po = temp.OrderBy(comparer).ToList();
                var no = newList.OrderBy(comparer).ToList();


                for (var i = 0; i < count; i++)
                {
                    var p = po[i];
                    var n = no[i];

                    if (!p.Equals(n))
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }
    }
}