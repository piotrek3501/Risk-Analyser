using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [Display(Name ="Nazwa pliku")]   
        public string FileName { get; set; }

        [Required]
        [Display(Name = "Zawartość pliku")]
        public byte[] FileData { get; set; }

    }
}
