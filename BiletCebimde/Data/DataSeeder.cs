using BiletCebimde.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BiletCebimde.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // Rolleri oluştur
            string[] roleNames = { "Admin", "Organizer", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin kullanıcısı oluştur
            var adminEmail = "admin@biletcebimde.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new Users
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Ahmet Yılmaz",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Organizer kullanıcısı oluştur
            var organizerEmail = "organizer@biletcebimde.com";
            var organizerUser = await userManager.FindByEmailAsync(organizerEmail);
            if (organizerUser == null)
            {
                organizerUser = new Users
                {
                    UserName = organizerEmail,
                    Email = organizerEmail,
                    FullName = "Mehmet Demir",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(organizerUser, "Organizer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(organizerUser, "Organizer");
                }
            }

            // Kategorileri oluştur
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Tiyatro" },
                    new Category { Name = "Konser" },
                    new Category { Name = "Opera" },
                    new Category { Name = "Bale" },
                    new Category { Name = "Stand-up" },
                    new Category { Name = "Festival" },
                    new Category { Name = "Spor" },
                    new Category { Name = "Çocuk Etkinliği" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Mekanları oluştur
            if (!context.Venues.Any())
            {
                var venues = new[]
                {
                    new Venue { Name = "Zorlu Center PSM", Address = "Zorlu Center, Beşiktaş, İstanbul" },
                    new Venue { Name = "İstanbul Şehir Tiyatroları", Address = "Kadıköy, İstanbul" },
                    new Venue { Name = "Bostancı Gösteri Merkezi", Address = "Bostancı, Kadıköy, İstanbul" },
                    new Venue { Name = "Ankara Devlet Opera ve Balesi", Address = "Ulus, Ankara" },
                    new Venue { Name = "İzmir Sanat", Address = "Konak, İzmir" },
                    new Venue { Name = "Volkswagen Arena", Address = "Ümraniye, İstanbul" },
                    new Venue { Name = "KüçükÇiftlik Park", Address = "Nişantaşı, İstanbul" },
                    new Venue { Name = "Cemal Reşit Rey Konser Salonu", Address = "Harbiye, Şişli, İstanbul" }
                };
                context.Venues.AddRange(venues);
                await context.SaveChangesAsync();
            }

            // Etkinlikleri oluştur (organizer kullanıcısı varsa)
            if (!context.Events.Any() && organizerUser != null)
            {
                var categories = context.Categories.ToList();
                var venues = context.Venues.ToList();

                if (categories.Any() && venues.Any())
                {
                    var events = new[]
                    {
                        new Event
                        {
                            Title = "Kral Lear - William Shakespeare",
                            Description = "Shakespeare'in en büyük trajedilerinden biri olan Kral Lear, İstanbul Şehir Tiyatroları'nın muhteşem yorumuyla sahnede. Yaşlı kralın hikayesi, güç, ihanet ve aile bağları üzerine derin bir yolculuk sunuyor.",
                            Date = DateTime.Now.AddDays(15).AddHours(20),
                            Capacity = 500,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Tiyatro").Id,
                            VenueId = venues.First(v => v.Name == "İstanbul Şehir Tiyatroları").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "Rock Konseri - Duman",
                            Description = "Türk rock müziğinin efsane grubu Duman, unutulmaz şarkılarıyla sahne alıyor. Canlı performans ve enerji dolu bir gece sizleri bekliyor!",
                            Date = DateTime.Now.AddDays(20).AddHours(21),
                            Capacity = 3000,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Konser").Id,
                            VenueId = venues.First(v => v.Name == "Volkswagen Arena").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "La Traviata - Giuseppe Verdi",
                            Description = "Verdi'nin en sevilen operalarından La Traviata, Ankara Devlet Opera ve Balesi'nin profesyonel kadrosu tarafından sahneleniyor. Mükemmel sesler ve görsel şölen!",
                            Date = DateTime.Now.AddDays(25).AddHours(19),
                            Capacity = 800,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Opera").Id,
                            VenueId = venues.First(v => v.Name == "Ankara Devlet Opera ve Balesi").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "Kuğu Gölü - Pyotr İlyiç Çaykovski",
                            Description = "Dünyaca ünlü bale eseri Kuğu Gölü, muhteşem koreografisi ve profesyonel dansçılarıyla büyülüyor. Klasik müzik ve dansın mükemmel uyumu!",
                            Date = DateTime.Now.AddDays(30).AddHours(20),
                            Capacity = 600,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Bale").Id,
                            VenueId = venues.First(v => v.Name == "İzmir Sanat").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "Cem Yılmaz Stand-up Gösterisi",
                            Description = "Türkiye'nin en sevilen komedyenlerinden Cem Yılmaz, yeni gösterisiyle sahnede! Mizah dolu bir akşam için kaçırılmayacak bir fırsat.",
                            Date = DateTime.Now.AddDays(12).AddHours(20),
                            Capacity = 2000,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Stand-up").Id,
                            VenueId = venues.First(v => v.Name == "Zorlu Center PSM").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "Jazz Festivali - İstanbul",
                            Description = "Uluslararası caz sanatçıları İstanbul'da buluşuyor! 3 gün sürecek festivalde dünyaca ünlü müzisyenler sahne alacak.",
                            Date = DateTime.Now.AddDays(40).AddHours(18),
                            Capacity = 5000,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Festival").Id,
                            VenueId = venues.First(v => v.Name == "KüçükÇiftlik Park").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "Çocuk Tiyatrosu - Pamuk Prenses",
                            Description = "Klasik masal Pamuk Prenses, çocuklar için özel olarak hazırlanmış interaktif tiyatro gösterisi olarak sahneleniyor. Tüm aile için eğlenceli bir deneyim!",
                            Date = DateTime.Now.AddDays(8).AddHours(14),
                            Capacity = 300,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Çocuk Etkinliği").Id,
                            VenueId = venues.First(v => v.Name == "Bostancı Gösteri Merkezi").Id,
                            OrganizerId = organizerUser.Id
                        },
                        new Event
                        {
                            Title = "Klasik Müzik Konseri - İstanbul Senfoni",
                            Description = "İstanbul Senfoni Orkestrası, Beethoven ve Mozart'ın en sevilen eserlerini seslendiriyor. Klasik müzikseverler için unutulmaz bir akşam!",
                            Date = DateTime.Now.AddDays(18).AddHours(20),
                            Capacity = 1200,
                            RegisteredCount = 0,
                            CategoryId = categories.First(c => c.Name == "Konser").Id,
                            VenueId = venues.First(v => v.Name == "Cemal Reşit Rey Konser Salonu").Id,
                            OrganizerId = organizerUser.Id
                        }
                    };
                    context.Events.AddRange(events);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

