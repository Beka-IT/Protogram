using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class User
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId {get;set;}
        public string Login {get;set;}
        public string Password {get;set;}
        public string Fullname {get;set;}
        public string Email {get;set;}
        public string PhoneNumber {get;set;}
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public LoginModel LoginModel {get;set;}
    }
}