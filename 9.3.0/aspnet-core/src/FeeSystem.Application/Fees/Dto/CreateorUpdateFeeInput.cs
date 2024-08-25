using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeeSystem.Fees.Dto
{
    public class CreateOrUpdateFeeInput
    {
        public int? FeeId { get; set; }

        [Required]
        public string FeeHeading { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int FeeTypeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int PaymentPlanId { get; set; }


    }
}
