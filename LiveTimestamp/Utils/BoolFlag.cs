using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTimestamp.Utils
{
    public interface IBoolFlagTaker
    {
        public bool TakeFlag();
    }

    public class BoolFlag : IBoolFlagTaker
    {
        private bool _flag = false;
        public bool Flag => _flag;

        public void UpFlag()
        {
            _flag = true;
        }

        public void Clear()
        {
            _flag = false;
        }

        public bool TakeFlag()
        {
            bool result = _flag;
            _flag = false;
            return result;
        }
    }
}
