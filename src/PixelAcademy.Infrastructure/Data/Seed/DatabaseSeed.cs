using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Infrastructure.Data.Seed;

public static class DatabaseSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, IDateTimeProvider dateTimeProvider)
    {
        if (!context.Users.Any())
        {
            var admin = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                PhoneNumber = "01000000001",
                Username = "admin",
                PasswordHash = passwordHasher.HashPassword("Admin123!"),
                FullName = "System Administrator",
                FirstName = "System",
                LastName = "Administrator",
                ParentPhoneNumber = "01000000001",
                Governorate = "Cairo",
                Address = "Admin Office",
                SchoolName = "PixelAcademy",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var instructor = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                PhoneNumber = "01000000002",
                Username = "instructor",
                PasswordHash = passwordHasher.HashPassword("Instructor123!"),
                FullName = "John Doe",
                FirstName = "John",
                LastName = "Doe",
                ParentPhoneNumber = "01000000002",
                Governorate = "Alexandria",
                Address = "Instructor Building",
                SchoolName = "PixelAcademy",
                Role = UserRole.Teacher, // 🚀 التعديل هنا: بقت Teacher
                IsActive = true,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var student = new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                PhoneNumber = "01000000003",
                Username = "student",
                PasswordHash = passwordHasher.HashPassword("Student123!"),
                FullName = "Jane Smith",
                FirstName = "Jane",
                LastName = "Smith",
                ParentPhoneNumber = "01000000004",
                Governorate = "Giza",
                Address = "Student Dorm",
                SchoolName = "PixelAcademy High",
                Role = UserRole.Student,
                IsActive = true,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Users.AddRangeAsync(admin, instructor, student);
            await context.SaveChangesAsync();

            var course = new Course
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Introduction to .NET 8",
                Description = "Learn the fundamentals of .NET 8 development including C#, ASP.NET Core, and Entity Framework.",
                ShortDescription = "Master .NET 8 fundamentals",
                Level = CourseLevel.Beginner,
                Status = CourseStatus.Published,
                Price = 49.99m,
                Category = "Programming",
                Tags = "dotnet,csharp,aspnet",
                InstructorId = instructor.Id,
                DurationMinutes = 360,
                CreatedAt = dateTimeProvider.UtcNow,
                CreatedBy = instructor.PhoneNumber
            };

            var course2 = new Course
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Title = "Advanced Clean Architecture",
                Description = "Deep dive into Clean Architecture, CQRS, MediatR, and Domain-Driven Design patterns.",
                ShortDescription = "Master enterprise architecture patterns",
                Level = CourseLevel.Advanced,
                Status = CourseStatus.Published,
                Price = 89.99m,
                Category = "Architecture",
                Tags = "clean-architecture,cqrs,ddd",
                InstructorId = instructor.Id,
                DurationMinutes = 480,
                CreatedAt = dateTimeProvider.UtcNow,
                CreatedBy = instructor.PhoneNumber
            };

            await context.Courses.AddRangeAsync(course, course2);
            await context.SaveChangesAsync();

            var lecture1 = new Lecture
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Title = "Getting Started with .NET 8",
                Description = "Overview of .NET 8 ecosystem and tooling.",
                OrderIndex = 1,
                DurationMinutes = 45,
                IsPreview = true,
                CourseId = course.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var lecture2 = new Lecture
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Title = "C# 12 New Features",
                Description = "Explore the latest C# 12 language features.",
                OrderIndex = 2,
                DurationMinutes = 60,
                IsPreview = false,
                CourseId = course.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Lectures.AddRangeAsync(lecture1, lecture2);
            await context.SaveChangesAsync();

            var contentItem1 = new ContentItem
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Title = "Introduction Video",
                Description = "Welcome video for the course.",
                OrderIndex = 1,
                Type = ContentItemType.Video,
                IsRequired = true,
                DurationSeconds = 300,
                LectureId = lecture1.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var contentItem2 = new ContentItem
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888889"),
                Title = "Course Syllabus PDF",
                Description = "PDF with full course syllabus.",
                OrderIndex = 2,
                Type = ContentItemType.PDF,
                IsRequired = false,
                LectureId = lecture1.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var contentItem3 = new ContentItem
            {
                Id = Guid.Parse("88888888-8888-8888-8888-88888888888A"),
                Title = "Week 1 Homework",
                Description = "First homework assignment.",
                OrderIndex = 3,
                Type = ContentItemType.Homework,
                IsRequired = true,
                LectureId = lecture1.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var contentItem4 = new ContentItem
            {
                Id = Guid.Parse("88888888-8888-8888-8888-88888888888B"),
                Title = "Mid-Lecture Quiz",
                Description = "Quick knowledge check.",
                OrderIndex = 4,
                Type = ContentItemType.Exam,
                IsRequired = true,
                LectureId = lecture1.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.ContentItems.AddRangeAsync(contentItem1, contentItem2, contentItem3, contentItem4);
            await context.SaveChangesAsync();

            var walletCode = new ActivationCode
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Code = "PA-WALLET-TEST01",
                Type = CodeType.WalletCredit,
                Value = 100.00m,
                MaxRedemptions = 1,
                CurrentRedemptions = 0,
                ExpiresAt = dateTimeProvider.UtcNow.AddDays(30),
                IsActive = true,
                GeneratedById = admin.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var enrollmentCode = new ActivationCode
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAB"),
                Code = "PA-ENROLL-TEST01",
                Type = CodeType.CourseEnrollment,
                CourseId = course.Id,
                MaxRedemptions = 1,
                CurrentRedemptions = 0,
                ExpiresAt = dateTimeProvider.UtcNow.AddDays(30),
                IsActive = true,
                GeneratedById = instructor.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var lectureCode = new ActivationCode
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAC"),
                Code = "PA-LECTURE-TEST01",
                Type = CodeType.LectureAccess,
                LectureId = lecture2.Id,
                MaxRedemptions = 1,
                CurrentRedemptions = 0,
                ExpiresAt = dateTimeProvider.UtcNow.AddDays(30),
                IsActive = true,
                GeneratedById = instructor.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var expiredCode = new ActivationCode
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAD"),
                Code = "PA-EXPIRED-TEST01",
                Type = CodeType.WalletCredit,
                Value = 50.00m,
                MaxRedemptions = 1,
                CurrentRedemptions = 0,
                ExpiresAt = dateTimeProvider.UtcNow.AddDays(-1),
                IsActive = true,
                GeneratedById = admin.Id,
                CreatedAt = dateTimeProvider.UtcNow.AddDays(-2)
            };

            var disabledCode = new ActivationCode
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAE"),
                Code = "PA-DISABLED-TEST01",
                Type = CodeType.WalletCredit,
                Value = 25.00m,
                MaxRedemptions = 1,
                CurrentRedemptions = 0,
                ExpiresAt = dateTimeProvider.UtcNow.AddDays(30),
                IsActive = false,
                GeneratedById = admin.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            var multiRedeemCode = new ActivationCode
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAF"),
                Code = "PA-MULTI-TEST01",
                Type = CodeType.WalletCredit,
                Value = 10.00m,
                MaxRedemptions = 3,
                CurrentRedemptions = 0,
                ExpiresAt = dateTimeProvider.UtcNow.AddDays(30),
                IsActive = true,
                GeneratedById = admin.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.ActivationCodes.AddRangeAsync(walletCode, enrollmentCode, lectureCode, expiredCode, disabledCode, multiRedeemCode);
            await context.SaveChangesAsync();

            // Seed Exam
            var exam = new Exam
            {
                Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Title = ".NET 8 Fundamentals Exam",
                Description = "Test your knowledge of .NET 8 basics.",
                Type = ExamType.CourseExam,
                CourseId = course.Id,
                DurationMinutes = 30,
                AttemptLimit = 2,
                PassScorePercent = 60,
                IsPublished = true,
                IsRandomized = false,
                ShowCorrectAnswers = true,
                CreatedById = instructor.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Exams.AddAsync(exam);
            await context.SaveChangesAsync();

            var q1 = new Question
            {
                Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC1"),
                ExamId = exam.Id,
                Text = "What is the primary language used in .NET development?",
                Type = QuestionType.MultipleChoice,
                OrderIndex = 1,
                Points = 10,
                CreatedAt = dateTimeProvider.UtcNow
            };
            q1.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD1"), Text = "C#", OrderIndex = 1, IsCorrect = true, CreatedAt = dateTimeProvider.UtcNow });
            q1.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD2"), Text = "Java", OrderIndex = 2, IsCorrect = false, CreatedAt = dateTimeProvider.UtcNow });
            q1.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD3"), Text = "Python", OrderIndex = 3, IsCorrect = false, CreatedAt = dateTimeProvider.UtcNow });

            var q2 = new Question
            {
                Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC2"),
                ExamId = exam.Id,
                Text = "ASP.NET Core uses the MVC pattern.",
                Type = QuestionType.TrueFalse,
                OrderIndex = 2,
                Points = 10,
                CreatedAt = dateTimeProvider.UtcNow
            };
            q2.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD4"), Text = "True", OrderIndex = 1, IsCorrect = true, CreatedAt = dateTimeProvider.UtcNow });
            q2.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD5"), Text = "False", OrderIndex = 2, IsCorrect = false, CreatedAt = dateTimeProvider.UtcNow });

            var q3 = new Question
            {
                Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC3"),
                ExamId = exam.Id,
                Text = "Which of the following are value types in C#?",
                Type = QuestionType.MultiSelect,
                OrderIndex = 3,
                Points = 10,
                CreatedAt = dateTimeProvider.UtcNow
            };
            q3.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD6"), Text = "int", OrderIndex = 1, IsCorrect = true, CreatedAt = dateTimeProvider.UtcNow });
            q3.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD7"), Text = "string", OrderIndex = 2, IsCorrect = false, CreatedAt = dateTimeProvider.UtcNow });
            q3.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD8"), Text = "bool", OrderIndex = 3, IsCorrect = true, CreatedAt = dateTimeProvider.UtcNow });
            q3.Options.Add(new QuestionOption { Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD9"), Text = "double", OrderIndex = 4, IsCorrect = true, CreatedAt = dateTimeProvider.UtcNow });

            var q4 = new Question
            {
                Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC4"),
                ExamId = exam.Id,
                Text = "Explain the difference between REST and SOAP.",
                Type = QuestionType.ShortAnswer,
                OrderIndex = 4,
                Points = 10,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Questions.AddRangeAsync(q1, q2, q3, q4);
            await context.SaveChangesAsync();

            // Seed Assignment
            var assignment = new Assignment
            {
                Id = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                Title = "Build a Simple API",
                Description = "Create a minimal API with two endpoints.",
                Instructions = "Use ASP.NET Core Minimal APIs to create GET and POST endpoints.",
                CourseId = course.Id,
                DueDate = dateTimeProvider.UtcNow.AddDays(7),
                MaxPoints = 100,
                AllowLateSubmission = false,
                IsPublished = true,
                CreatedById = instructor.Id,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Assignments.AddAsync(assignment);
            await context.SaveChangesAsync();

            // Seed Notification for student
            var notification = new Notification
            {
                Id = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
                UserId = student.Id,
                Title = "Welcome to PixelAcademy!",
                Message = "You have been enrolled in .NET 8 Fundamentals course.",
                Type = NotificationType.EnrollmentUpdate,
                Status = NotificationStatus.Unread,
                RelatedEntityId = course.Id,
                RelatedEntityType = "Course",
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            // Seed Announcement
            var announcement = new Announcement
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Title = "Platform Update",
                Content = "We have added new exam and assignment features to the platform.",
                Target = AnnouncementTarget.All,
                CreatedById = admin.Id,
                IsPublished = true,
                PublishedAt = dateTimeProvider.UtcNow,
                Priority = 1,
                CreatedAt = dateTimeProvider.UtcNow
            };

            await context.Announcements.AddAsync(announcement);
            await context.SaveChangesAsync();

            // Seed Audit Logs
            var auditLogs = new List<AuditLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = admin.Id,
                    Action = AuditActionType.Login,
                    EntityType = "User",
                    EntityId = admin.Id,
                    Details = "Admin logged in",
                    IsAdminAction = true,
                    CreatedAt = dateTimeProvider.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = instructor.Id,
                    Action = AuditActionType.CreateCourse,
                    EntityType = "Course",
                    EntityId = course.Id,
                    Details = "Created course: .NET 8 Fundamentals",
                    IsAdminAction = false,
                    CreatedAt = dateTimeProvider.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = student.Id,
                    Action = AuditActionType.Register,
                    EntityType = "User",
                    EntityId = student.Id,
                    Details = "Student registered",
                    IsAdminAction = false,
                    CreatedAt = dateTimeProvider.UtcNow
                }
            };

            await context.AuditLogs.AddRangeAsync(auditLogs);
            await context.SaveChangesAsync();
        }
    }
}