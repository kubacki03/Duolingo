using Duolingo.Areas.Identity.Data;

namespace Duolingo.Models
{
    public class ExperienceReward
    {
        public long Id { get; set; }
        public int DoneTasks { get; set; }
        public string RewardName { get; set; }

    }
}
