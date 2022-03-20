using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TouchRule
    {
        public int TouchRuleId { get; set; }

        [MaxLength(20)]
        public string Rule { get; set; } = default!;

        public ICollection<Config>? Configs { get; set; } = default!;
    }
}