using ModelEntites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
namespace ProxyLibrary
{

 
    public class ReadonlyStateFromTail
    {
        public ReadonlyStateFromTail(string relationshipTail)
        {
            RelationshipTail = relationshipTail;
        }
        public string RelationshipTail { set; get; }

    }

    
}
