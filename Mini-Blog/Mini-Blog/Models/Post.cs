using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniBlog.Models
{
    /// <summary>
    /// Model, in welches die DB Dateien gespeichert werden, welche später im Dashboard angezeigt werden.
    /// </summary>
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }  
        public string Content { get; set; }
        public DateTime Createdon { get; set; }
        public DateTime Modifiedon { get; set; }
    }
}