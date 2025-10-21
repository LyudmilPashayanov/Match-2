using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeTwo
{
    public interface IEventOrderClaimed : IEventBusSubscriber
    {
        void OrderClaimed(int id);
    }
}
