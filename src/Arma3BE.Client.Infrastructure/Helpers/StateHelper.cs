using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Client.Infrastructure.Helpers
{
    public class StateHelper<T> where T : class
    {
        private List<T> _previousRequest;

        protected bool HaveChanges<TK>(List<T> newList, Func<T, TK> orderer, Func<T, T, bool> comparer = null)
        {
            var temp = _previousRequest;
            _previousRequest = newList;

            if (temp != null && temp.Count == newList.Count)
            {
                var count = temp.Count;
                var po = temp.OrderBy(orderer).ToList();
                var no = newList.OrderBy(orderer).ToList();


                for (var i = 0; i < count; i++)
                {
                    var p = po[i];
                    var n = no[i];

                    if (comparer != null)
                    {
                        if (!comparer(p, n))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (!p.Equals(n))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            return true;
        }
    }
}