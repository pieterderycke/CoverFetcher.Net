using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoverFetcher.Messages
{
    public class ShowErrorMessage : MessageBase
    {
        public string Message { get; set; }
    }
}
