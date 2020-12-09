using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBoardv3.DATA.EF.Metadata
{
    class JobBoardMetadata
    {
        public class positionMetaData
        {
            [Required(ErrorMessage = "* Title is required")]
            [StringLength(20, ErrorMessage = "* Title must be less than 50 characters")]
            [Display(Name = "Title")]
            public string Title { get; set; }

            [DisplayFormat(NullDisplayText = "N/A")]
            [Display(Name = "Job Description")]
            public string JobDescription { get; set; }
        }

        [MetadataType(typeof(positionMetaData))]
        public partial class position
        {

        }

        public class UserDetailsMetaData
        {
            [Required(ErrorMessage = "* First Name is required")]
            [StringLength(50, ErrorMessage = "* First Name must be less than 50 characters")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "* Last Name is required")]
            [StringLength(50, ErrorMessage = "* Last Name must be less than 50 characters")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [StringLength(75, ErrorMessage = "* Resume Url must be less than 75 characters")]
            [DisplayFormat(NullDisplayText = "N/A")]
            [DataType(DataType.ImageUrl, ErrorMessage = "* Please enter a resume url")]
            [Display(Name = "Resume Url")]
            public string ResumeFilename { get; set; }
        }

        [MetadataType(typeof(UserDetailsMetaData))]
        public partial class UserDetails
        {
            //[Display(Name = "Full Name")]
            //public string FullName { get { return FirstName + " " + LastName; } }
        }

        public class OpenPositionMetaData
        {

        }

        [MetadataType(typeof(OpenPositionMetaData))]
        public partial class OpenPosition
        {

        }

        public class LocationMetaData
        {
            [Required(ErrorMessage = "* Location Number is required")]
            [Display(Name = "Location Number")]
            public string StoreNumber { get; set; }

            [Display(Name = "City Name")]
            [StringLength(50, ErrorMessage = "* City must be less than 50 characters")]
            public string City { get; set; }

            [Display(Name = "State")]
            [StringLength(2, ErrorMessage = "* State must be less than 2 characters")]
            public string State { get; set; }
        }

        [MetadataType(typeof(LocationMetaData))]
        public partial class Location
        {
            //public string LocationInfo { get { return CityName + " " + State + " " + StoreNumber} }
        }

        public class ApplicationMetaData
        {
            [Required(ErrorMessage = "* Application Date is Required")]
            [Display(Name = "Application Date")]
            [DisplayFormat(DataFormatString = "{0:d}")]
            public System.DateTime ApplicationDate { get; set; }

            [StringLength(2000, ErrorMessage = "* Resume Url must be less than 2000 characters")]
            [DisplayFormat(NullDisplayText = "N/A")]
            [DataType(DataType.ImageUrl, ErrorMessage = "* Please enter a resume url")]
            public string ManagerNotes { get; set; }

            [Required(ErrorMessage = "* Application Status is Required")]
            [Display(Name = "Application Status")]
            public int ApplicationStatus { get; set; }

            [StringLength(75, ErrorMessage = "* Resume Url must be less than 75 characters")]
            [DisplayFormat(NullDisplayText = "N/A")]
            [DataType(DataType.ImageUrl, ErrorMessage = "* Please enter a resume url")]
            [Display(Name = "Resume Url")]
            public string ResumeFileName { get; set; }
        }

        public class ApplicationStatuMetaData
        {
            [Display(Name = "Status Name")]
            [StringLength(50, ErrorMessage = "* City must be less than 50 characters")]
            public string StatusName { get; set; }

            [Display(Name = "Status Description")]
            [StringLength(250, ErrorMessage = "* City must be less than 250 characters")]
            public string StatusDescription { get; set; }
        }

        [MetadataType(typeof(ApplicationStatuMetaData))]
        public partial class ApplicationStatu
        {

        }
    }
}
