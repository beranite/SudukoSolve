using System;
using System.Linq;
using System.Collections.Generic;

namespace Suduko
{
  public class CellListElementException : Exception
  {
        public CellListElementException(string message, Exception innerException = null ) : 
          base(message, innerException)
        {            
        }
    }
}
