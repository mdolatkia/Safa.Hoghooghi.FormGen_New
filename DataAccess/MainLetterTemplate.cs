//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class MainLetterTemplate
    {
        public int ID { get; set; }
        public string FileExtension { get; set; }
        public short Type { get; set; }
        public byte[] Content { get; set; }
    
        public virtual LetterTemplate LetterTemplate { get; set; }
    }
}
