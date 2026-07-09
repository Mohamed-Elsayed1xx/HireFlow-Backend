using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence;

/// <summary>
/// Populates realistic demo data (jobs, candidates, applications, interviews,
/// evaluations, team members, and a couple of extra companies) so the
/// dashboard, Pipeline board, Analytics charts, and Admin portal aren't empty
/// on a freshly-registered company.
///
/// This is demo/dev-only scaffolding, not part of the real product flow —
/// call it from Program.cs only, and only in Development. It is idempotent:
/// it checks for its own marker job ("Senior Backend Engineer") before doing
/// anything, so re-running it (e.g. every `dotnet run`) is a no-op after the
/// first successful pass.
/// </summary>
public static class DemoDataSeeder
{
    private static readonly string[] DemoSkillPool =
    {
        "C#", ".NET", "Angular", "TypeScript", "SQL", "React", "Figma",
        "User Research", "SEO", "Content Strategy", "AWS", "Docker",
        "Kubernetes", "CI/CD", "Salesforce", "Account Management",
    };

    public static async Task SeedAsync(AppDbContext context, IPasswordService passwordService)
    {
        // Idempotency guard: check for OUR OWN demo job specifically, not
        // "any candidate exists" — the target company already has a real
        // candidate account registered against it from manual testing, so
        // a broader check would wrongly skip seeding forever.
        const string demoMarkerJobTitle = "Senior Backend Engineer";
        if (await context.Jobs.AnyAsync(j => j.Title == demoMarkerJobTitle && j.Department == "Engineering"))
            return;

        var company = await context.Companies.OrderBy(c => c.CreatedAt).FirstOrDefaultAsync();
        if (company is null)
            return; // no company registered yet, nothing to attach demo data to

        var companyAdmin = await context.Users
            .Where(u => u.CompanyId == company.Id && u.Role == UserRole.CompanyAdmin)
            .OrderBy(u => u.CreatedAt)
            .FirstOrDefaultAsync();
        if (companyAdmin is null)
            return;

        var now = DateTime.UtcNow;
        var demoPasswordHash = passwordService.Hash("Demo@123456");

        // ---------------------------------------------------------------
        // 1. Two more team members on the real company, so Team / Interviews
        //    / assignees have more than one person to show.
        // ---------------------------------------------------------------
        var hrManager = new User
        {
            CompanyId = company.Id,
            FirstName = "Nour",
            LastName = "Hassan",
            Email = "nour.hassan.demo@hireflow.com",
            PasswordHash = demoPasswordHash,
            Role = UserRole.HRManager,
            IsActive = true,
        };
        var hiringManager = new User
        {
            CompanyId = company.Id,
            FirstName = "Khaled",
            LastName = "Ibrahim",
            Email = "khaled.ibrahim.demo@hireflow.com",
            PasswordHash = demoPasswordHash,
            Role = UserRole.HiringManager,
            IsActive = true,
        };
        await context.Users.AddRangeAsync(hrManager, hiringManager);

        // ---------------------------------------------------------------
        // 2. Jobs across a few departments/statuses/types.
        // ---------------------------------------------------------------
        var jobBackend = new Job
        {
            CompanyId = company.Id,
            CreatedById = companyAdmin.Id,
            Title = "Senior Backend Engineer",
            Department = "Engineering",
            Location = "Cairo, Egypt",
            Type = JobType.FullTime,
            ExperienceLevel = ExperienceLevel.Senior,
            SalaryMin = 40000,
            SalaryMax = 60000,
            Description = "Own the core API platform, mentor mid-level engineers, and drive architecture decisions for our .NET backend.",
            Requirements = "5+ years with C#/.NET, strong SQL, experience with Clean Architecture and CQRS.",
            Status = JobStatus.Active,
        };
        var jobDesigner = new Job
        {
            CompanyId = company.Id,
            CreatedById = companyAdmin.Id,
            Title = "Product Designer",
            Department = "Design",
            Location = "Remote",
            Type = JobType.Contract,
            ExperienceLevel = ExperienceLevel.Mid,
            SalaryMin = 25000,
            SalaryMax = 35000,
            Description = "Design end-to-end product flows for our recruitment platform, working closely with engineering and the founders.",
            Requirements = "3+ years product design, strong Figma portfolio, comfortable with design systems.",
            Status = JobStatus.Active,
        };
        var jobMarketing = new Job
        {
            CompanyId = company.Id,
            CreatedById = companyAdmin.Id,
            Title = "Marketing Specialist",
            Department = "Marketing",
            Location = "Cairo, Egypt",
            Type = JobType.FullTime,
            ExperienceLevel = ExperienceLevel.Junior,
            SalaryMin = 12000,
            SalaryMax = 18000,
            Description = "Run our content calendar, SEO efforts, and support demand-gen campaigns.",
            Requirements = "1-2 years marketing experience, strong writing skills.",
            Status = JobStatus.Paused,
        };
        var jobDevOps = new Job
        {
            CompanyId = company.Id,
            CreatedById = companyAdmin.Id,
            Title = "DevOps Engineer",
            Department = "Engineering",
            Location = "Remote",
            Type = JobType.FullTime,
            ExperienceLevel = ExperienceLevel.Senior,
            SalaryMin = 45000,
            SalaryMax = 65000,
            Description = "Still being scoped with the engineering team — not published yet.",
            Requirements = "AWS, Docker, Kubernetes, CI/CD pipelines.",
            Status = JobStatus.Draft, // intentionally draft: exercises "Total jobs" without inflating "Active jobs"
        };
        var jobSuccess = new Job
        {
            CompanyId = company.Id,
            CreatedById = companyAdmin.Id,
            Title = "Customer Success Manager",
            Department = "Operations",
            Location = "Cairo, Egypt",
            Type = JobType.FullTime,
            ExperienceLevel = ExperienceLevel.Mid,
            SalaryMin = 18000,
            SalaryMax = 26000,
            Description = "This role has been filled — kept here as a closed requisition for history.",
            Requirements = "Account management background, Arabic/English bilingual.",
            Status = JobStatus.Closed,
        };
        await context.Jobs.AddRangeAsync(jobBackend, jobDesigner, jobMarketing, jobDevOps, jobSuccess);

        await context.JobAssignees.AddRangeAsync(
            new JobAssignee { JobId = jobBackend.Id, UserId = hiringManager.Id, Role = "Hiring Manager" },
            new JobAssignee { JobId = jobDesigner.Id, UserId = hrManager.Id, Role = "Recruiter" }
        );

        // ---------------------------------------------------------------
        // 3. Candidates + profiles (skills pulled from a small shared pool).
        // ---------------------------------------------------------------
        var names = new (string First, string Last, string Title, string Location)[]
        {
            ("Sara",    "Ahmed",   "Backend Developer",        "Cairo, Egypt"),
            ("Youssef", "Mahmoud", ".NET Engineer",            "Giza, Egypt"),
            ("Nourhan", "Tarek",   "Senior Software Engineer", "Cairo, Egypt"),
            ("Omar",    "Khaled",  "Backend Lead",             "Alexandria, Egypt"),
            ("Laila",   "Hassan",  "Software Engineer",        "Cairo, Egypt"),
            ("Karim",   "Fathy",   "Product Designer",         "Remote"),
            ("Dina",    "Samir",   "UI/UX Designer",           "Cairo, Egypt"),
            ("Ahmed",   "Nabil",   "Senior Product Designer",  "Remote"),
            ("Mariam",  "Adel",    "UX Researcher",            "Cairo, Egypt"),
            ("Hassan",  "Ibrahim", "Marketing Coordinator",    "Cairo, Egypt"),
            ("Yasmin",  "Fouad",   "Content Marketer",         "Mansoura, Egypt"),
            ("Tarek",   "Anwar",   "Growth Marketer",          "Cairo, Egypt"),
            ("Salma",   "Hosny",   "Customer Success Lead",    "Cairo, Egypt"),
            ("Amr",     "Zaki",    "Account Manager",          "Alexandria, Egypt"),
        };

        var rng = new Random(42); // fixed seed: reproducible demo data, not security-sensitive
        var candidates = names.Select((n, i) => new Candidate
        {
            FirstName = n.First,
            LastName = n.Last,
            Email = $"{n.First.ToLower()}.{n.Last.ToLower()}.demo@example.com",
            Phone = $"+2010{rng.Next(10000000, 99999999)}",
            PasswordHash = demoPasswordHash,
            HeadlineTitle = n.Title,
            Location = n.Location,
        }).ToList();
        await context.Candidates.AddRangeAsync(candidates);

        var profiles = candidates.Select(c => new CandidateProfile
        {
            CandidateId = c.Id,
            // No CvUrl on purpose: these candidates never went through the
            // real upload flow, so there's no actual file on disk. Leaving
            // this null avoids a broken "download CV" button pointing at a
            // file that doesn't exist — only real signups have a CV to
            // download.
            CvUrl = null,
            Skills = DemoSkillPool.OrderBy(_ => rng.Next()).Take(4).ToList(),
            Summary = $"{c.HeadlineTitle} with hands-on experience shipping production work in fast-moving teams.",
        });
        await context.CandidateProfiles.AddRangeAsync(profiles);

        await context.SaveChangesAsync();

        // ---------------------------------------------------------------
        // 4. Applications, spread across stages/sources/months so the
        //    Pipeline board, Hiring Funnel, Source Breakdown, and
        //    Candidates-Over-Time charts all show something real.
        // ---------------------------------------------------------------
        JobApplication MakeApp(Job job, Candidate candidate, ApplicationStage stage, string? source, int createdDaysAgo, int? updatedDaysAgo = null)
        {
            var app = new JobApplication
            {
                JobId = job.Id,
                CandidateId = candidate.Id,
                Stage = stage,
                Source = source,
                CreatedAt = now.AddDays(-createdDaysAgo),
                RejectedAt = stage == ApplicationStage.Rejected ? now.AddDays(-(updatedDaysAgo ?? createdDaysAgo)) : null,
                RejectedById = stage == ApplicationStage.Rejected ? companyAdmin.Id : null,
            };
            if (updatedDaysAgo.HasValue)
                app.UpdatedAt = now.AddDays(-updatedDaysAgo.Value);
            return app;
        }

        var appSara    = MakeApp(jobBackend,  candidates[0],  ApplicationStage.Applied,   "LinkedIn",         3);
        var appYoussef = MakeApp(jobBackend,  candidates[1],  ApplicationStage.Screening, "Referral",         40);
        var appNourhan = MakeApp(jobBackend,  candidates[2],  ApplicationStage.Interview, "Company Website",  45);
        var appOmar    = MakeApp(jobBackend,  candidates[3],  ApplicationStage.Hired,     "LinkedIn",         70, updatedDaysAgo: 0);
        var appLaila   = MakeApp(jobBackend,  candidates[4],  ApplicationStage.Rejected,  "Referral",         100, updatedDaysAgo: 95);

        var appKarim   = MakeApp(jobDesigner, candidates[5],  ApplicationStage.Applied,   "LinkedIn",         5);
        var appDina    = MakeApp(jobDesigner, candidates[6],  ApplicationStage.Screening, null,               20);
        var appAhmedN  = MakeApp(jobDesigner, candidates[7],  ApplicationStage.Offer,     "Company Website",  35);
        var appMariam  = MakeApp(jobDesigner, candidates[8],  ApplicationStage.Rejected,  "LinkedIn",         60, updatedDaysAgo: 55);

        var appHassan  = MakeApp(jobMarketing, candidates[9],  ApplicationStage.Applied,  "Referral",         2);
        var appYasmin  = MakeApp(jobMarketing, candidates[10], ApplicationStage.Applied,  "LinkedIn",         8);
        var appTarek   = MakeApp(jobMarketing, candidates[11], ApplicationStage.Rejected, null,               90, updatedDaysAgo: 85);

        var appSalma   = MakeApp(jobSuccess, candidates[12], ApplicationStage.Hired,      "Referral",         150, updatedDaysAgo: 130);
        var appAmr     = MakeApp(jobSuccess, candidates[13], ApplicationStage.Rejected,   "LinkedIn",         110, updatedDaysAgo: 105);

        await context.JobApplications.AddRangeAsync(
            appSara, appYoussef, appNourhan, appOmar, appLaila,
            appKarim, appDina, appAhmedN, appMariam,
            appHassan, appYasmin, appTarek,
            appSalma, appAmr
        );
        await context.SaveChangesAsync();

        // ---------------------------------------------------------------
        // 5. Interviews + evaluations for the applications that reached
        //    that stage — populates the Interviews page and the
        //    "Interviews Scheduled" / average-time-to-hire KPIs.
        // ---------------------------------------------------------------
        var interviewNourhan = new Interview
        {
            ApplicationId = appNourhan.Id,
            ScheduledById = hiringManager.Id,
            Title = "Backend Engineer — Technical Interview",
            Type = InterviewType.Video,
            ScheduledAt = now.AddDays(3),
            DurationMinutes = 60,
            MeetingUrl = "https://meet.google.com/demo-interview",
            Status = InterviewStatus.Scheduled,
        };
        var interviewOmar = new Interview
        {
            ApplicationId = appOmar.Id,
            ScheduledById = hiringManager.Id,
            Title = "Backend Engineer — Onsite Final Round",
            Type = InterviewType.OnSite,
            ScheduledAt = now.AddDays(-10),
            DurationMinutes = 90,
            Location = "HireFlow HQ, Cairo",
            Status = InterviewStatus.Completed,
        };
        var interviewAhmedN = new Interview
        {
            ApplicationId = appAhmedN.Id,
            ScheduledById = hrManager.Id,
            Title = "Product Designer — Portfolio Review",
            Type = InterviewType.Video,
            ScheduledAt = now.AddDays(-6),
            DurationMinutes = 45,
            MeetingUrl = "https://meet.google.com/demo-interview-2",
            Status = InterviewStatus.Completed,
        };
        var interviewSalma = new Interview
        {
            ApplicationId = appSalma.Id,
            ScheduledById = companyAdmin.Id,
            Title = "Customer Success Manager — Final Interview",
            Type = InterviewType.OnSite,
            ScheduledAt = now.AddDays(-20),
            DurationMinutes = 60,
            Location = "HireFlow HQ, Cairo",
            Status = InterviewStatus.Completed,
        };
        await context.Interviews.AddRangeAsync(interviewNourhan, interviewOmar, interviewAhmedN, interviewSalma);

        await context.InterviewInterviewers.AddRangeAsync(
            new InterviewInterviewer { InterviewId = interviewNourhan.Id, UserId = hrManager.Id },
            new InterviewInterviewer { InterviewId = interviewNourhan.Id, UserId = hiringManager.Id },
            new InterviewInterviewer { InterviewId = interviewOmar.Id, UserId = hiringManager.Id },
            new InterviewInterviewer { InterviewId = interviewAhmedN.Id, UserId = hrManager.Id },
            new InterviewInterviewer { InterviewId = interviewSalma.Id, UserId = companyAdmin.Id }
        );

        await context.Evaluations.AddRangeAsync(
            new Evaluation
            {
                InterviewId = interviewOmar.Id,
                EvaluatorId = hiringManager.Id,
                Rating = 9,
                TechnicalScore = 9,
                CultureScore = 8,
                CommunicationScore = 9,
                Strengths = "Deep .NET/EF Core knowledge, clear communicator, strong system design instincts.",
                Weaknesses = "Limited exposure to our specific cloud stack, but ramped up fast in the take-home.",
                Recommendation = EvaluationRecommendation.StrongYes,
            },
            new Evaluation
            {
                InterviewId = interviewAhmedN.Id,
                EvaluatorId = hrManager.Id,
                Rating = 8,
                TechnicalScore = 8,
                CultureScore = 9,
                CommunicationScore = 8,
                Strengths = "Excellent portfolio, strong storytelling around design decisions.",
                Weaknesses = "Less experience with design systems at scale.",
                Recommendation = EvaluationRecommendation.Yes,
            },
            new Evaluation
            {
                InterviewId = interviewSalma.Id,
                EvaluatorId = companyAdmin.Id,
                Rating = 9,
                TechnicalScore = 7,
                CultureScore = 10,
                CommunicationScore = 9,
                Strengths = "Outstanding client-facing presence, proven track record retaining accounts.",
                Weaknesses = "Would benefit from more exposure to our product's technical side.",
                Recommendation = EvaluationRecommendation.StrongYes,
            }
        );

        // ---------------------------------------------------------------
        // 6. A few notifications for the company admin.
        // ---------------------------------------------------------------
        await context.Notifications.AddRangeAsync(
            new Notification
            {
                UserId = companyAdmin.Id,
                Title = "New application received",
                Body = "Sara Ahmed applied for Senior Backend Engineer.",
                Type = "application",
                IsRead = false,
            },
            new Notification
            {
                UserId = companyAdmin.Id,
                Title = "Interview scheduled",
                Body = "Nourhan Tarek's technical interview is booked for " + now.AddDays(3).ToString("MMM d"),
                Type = "interview",
                IsRead = false,
            },
            new Notification
            {
                UserId = companyAdmin.Id,
                Title = "Candidate hired",
                Body = "Omar Khaled accepted the offer for Senior Backend Engineer. 🎉",
                Type = "hire",
                IsRead = true,
            }
        );

        // ---------------------------------------------------------------
        // 7. Two extra companies so the Admin portal (Companies list,
        //    Recent signups, Active/Total company counts) isn't showing
        //    just the one company you registered by hand.
        // ---------------------------------------------------------------
        var growthPlan = await context.Plans.OrderBy(p => p.Price).Skip(1).FirstOrDefaultAsync()
                          ?? await context.Plans.FirstAsync();
        var starterPlan = await context.Plans.OrderBy(p => p.Price).FirstAsync();

        var novaRobotics = new Company
        {
            Name = "Nova Robotics",
            Slug = "nova-robotics",
            Industry = "Technology",
            Size = "11-50",
            PlanId = growthPlan.Id,
            IsActive = true,
            CreatedAt = now.AddDays(-25),
        };
        var bluewaveRetail = new Company
        {
            Name = "Bluewave Retail",
            Slug = "bluewave-retail",
            Industry = "Retail",
            Size = "51-200",
            PlanId = starterPlan.Id,
            IsActive = true,
            CreatedAt = now.AddDays(-8),
        };
        await context.Companies.AddRangeAsync(novaRobotics, bluewaveRetail);

        var novaAdmin = new User
        {
            CompanyId = novaRobotics.Id,
            FirstName = "Youssef",
            LastName = "Nova",
            Email = "admin.demo@novarobotics.example",
            PasswordHash = demoPasswordHash,
            Role = UserRole.CompanyAdmin,
            IsActive = true,
        };
        var bluewaveAdmin = new User
        {
            CompanyId = bluewaveRetail.Id,
            FirstName = "Rana",
            LastName = "Sobhy",
            Email = "admin.demo@bluewaveretail.example",
            PasswordHash = demoPasswordHash,
            Role = UserRole.CompanyAdmin,
            IsActive = true,
        };
        await context.Users.AddRangeAsync(novaAdmin, bluewaveAdmin);
        await context.SaveChangesAsync(); // need Ids for the jobs below

        await context.Jobs.AddRangeAsync(
            new Job
            {
                CompanyId = novaRobotics.Id,
                CreatedById = novaAdmin.Id,
                Title = "Machine Learning Engineer",
                Department = "Engineering",
                Location = "Cairo, Egypt",
                Type = JobType.FullTime,
                ExperienceLevel = ExperienceLevel.Senior,
                Description = "Build the perception stack for our warehouse robots.",
                Status = JobStatus.Active,
            },
            new Job
            {
                CompanyId = novaRobotics.Id,
                CreatedById = novaAdmin.Id,
                Title = "QA Engineer",
                Department = "Engineering",
                Location = "Cairo, Egypt",
                Type = JobType.FullTime,
                ExperienceLevel = ExperienceLevel.Mid,
                Description = "Own test automation across firmware and cloud services.",
                Status = JobStatus.Active,
            },
            new Job
            {
                CompanyId = bluewaveRetail.Id,
                CreatedById = bluewaveAdmin.Id,
                Title = "Store Operations Lead",
                Department = "Operations",
                Location = "Alexandria, Egypt",
                Type = JobType.FullTime,
                ExperienceLevel = ExperienceLevel.Mid,
                Description = "Lead day-to-day operations across our flagship stores.",
                Status = JobStatus.Active,
            },
            new Job
            {
                CompanyId = bluewaveRetail.Id,
                CreatedById = bluewaveAdmin.Id,
                Title = "Regional Sales Manager",
                Department = "Sales",
                Location = "Cairo, Egypt",
                Type = JobType.FullTime,
                ExperienceLevel = ExperienceLevel.Senior,
                Description = "Own sales targets across the greater Cairo region.",
                Status = JobStatus.Paused,
            }
        );

        await context.SaveChangesAsync();
    }
}
