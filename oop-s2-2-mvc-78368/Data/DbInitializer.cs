using oop_s2_2_mvc_78368.Models;

namespace oop_s2_2_mvc_78368.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Premises.Any())
                return; // DB already seeded

            var premisesList = new List<Premises>
        {
            new Premises { Name = "Cafe One", Address = "Street 1", Town = "Dublin", RiskRating = "Low" },
            new Premises { Name = "Burger Hub", Address = "Street 2", Town = "Dublin", RiskRating = "High" },
            new Premises { Name = "Pizza Spot", Address = "Street 3", Town = "Cork", RiskRating = "Medium" },
            new Premises { Name = "Sushi Bar", Address = "Street 4", Town = "Cork", RiskRating = "High" },
            new Premises { Name = "Vegan Place", Address = "Street 5", Town = "Galway", RiskRating = "Low" },
            new Premises { Name = "Grill House", Address = "Street 6", Town = "Galway", RiskRating = "Medium" },
            new Premises { Name = "Bakery Bliss", Address = "Street 7", Town = "Dublin", RiskRating = "Low" },
            new Premises { Name = "Taco Town", Address = "Street 8", Town = "Cork", RiskRating = "High" },
            new Premises { Name = "Deli Corner", Address = "Street 9", Town = "Galway", RiskRating = "Medium" },
            new Premises { Name = "Noodle Bar", Address = "Street 10", Town = "Dublin", RiskRating = "High" },
            new Premises { Name = "Seafood Shack", Address = "Street 11", Town = "Cork", RiskRating = "Medium" },
            new Premises { Name = "Steak House", Address = "Street 12", Town = "Galway", RiskRating = "High" }
        };

            context.Premises.AddRange(premisesList);
            context.SaveChanges();

            var inspections = new List<Inspection>();
            var rand = new Random();

            foreach (var p in premisesList)
            {
                for (int i = 0; i < 2; i++)
                {
                    var score = rand.Next(40, 100);
                    inspections.Add(new Inspection
                    {
                        PremisesId = p.Id,
                        InspectionDate = DateTime.Now.AddDays(-rand.Next(1, 60)),
                        Score = score,
                        Outcome = score > 60 ? "Pass" : "Fail",
                        Notes = "Routine inspection"
                    });
                }
            }

            context.Inspections.AddRange(inspections);
            context.SaveChanges();

            var followUps = new List<FollowUp>();

            foreach (var insp in inspections.Take(10))
            {
                followUps.Add(new FollowUp
                {
                    InspectionId = insp.Id,
                    DueDate = DateTime.Now.AddDays(rand.Next(-10, 10)),
                    Status = rand.Next(0, 2) == 0 ? "Open" : "Closed",
                    ClosedDate = rand.Next(0, 2) == 0 ? null : DateTime.Now
                });
            }

            context.FollowUps.AddRange(followUps);
            context.SaveChanges();
        }
    }
}
