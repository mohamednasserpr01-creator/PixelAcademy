using AutoMapper;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.DTOs.Analytics;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Application.DTOs.Enrollments;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Application.DTOs.Media;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Application.DTOs.Transactions;
using PixelAcademy.Application.DTOs.Users;
using PixelAcademy.Application.DTOs.Wallet;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.EducationalStageName, opt => opt.MapFrom(src => src.EducationalStage != null ? src.EducationalStage.Name : null))
            .ForMember(dest => dest.EducationStreamName, opt => opt.MapFrom(src => src.EducationStream != null ? src.EducationStream.Name : null));
        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.EducationalStageName, opt => opt.MapFrom(src => src.EducationalStage != null ? src.EducationalStage.Name : null))
            .ForMember(dest => dest.EducationStreamName, opt => opt.MapFrom(src => src.EducationStream != null ? src.EducationStream.Name : null));

        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.FullName : null))
            .ForMember(dest => dest.LectureCount, opt => opt.MapFrom(src => src.Lectures.Count))
            .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count));

        CreateMap<Course, CourseDetailDto>();

        CreateMap<Lecture, LectureDto>();

        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : null))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<VideoProgress, VideoProgressDto>()
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture.Title));

        CreateMap<MediaAsset, MediaAssetDto>();

        CreateMap<ContentItem, ContentItemDto>()
            .ForMember(dest => dest.MediaAssetUrl, opt => opt.MapFrom(src => src.MediaAsset != null ? src.MediaAsset.Url : null));

        CreateMap<ActivationCode, ActivationCodeDto>()
            .ForMember(dest => dest.GeneratedByName, opt => opt.MapFrom(src => src.GeneratedBy != null ? src.GeneratedBy.FullName : null))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null))
            .ForMember(dest => dest.IsFullyRedeemed, opt => opt.MapFrom(src => src.IsFullyRedeemed))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired));

        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ActivationCode != null ? src.ActivationCode.Code : null))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null));

        CreateMap<WalletTransaction, WalletTransactionDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ActivationCode != null ? src.ActivationCode.Code : null))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null));

        CreateMap<WatchSession, WatchSessionDto>()
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Lecture != null && src.Lecture.Course != null ? src.Lecture.Course.Title : null));

        CreateMap<VideoProgress, LectureProgressSummaryDto>()
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.DurationMinutes : 0))
            .ForMember(dest => dest.LastPositionSeconds, opt => opt.MapFrom(src => src.WatchedSeconds));

        // Exam mappings
        CreateMap<Exam, ExamDto>()
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null))
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
            .ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src => src.Questions.Sum(q => q.Points)));

        CreateMap<Exam, ExamDetailDto>();

        CreateMap<Question, QuestionDto>();
        CreateMap<QuestionOption, QuestionOptionDto>();
        CreateMap<QuestionOption, QuestionOptionDetailDto>();

        CreateMap<ExamAttempt, ExamAttemptDto>()
            .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Title : null));

        CreateMap<ExamAnswer, ExamAnswerDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question != null ? src.Question.Text : null))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Question != null ? src.Question.Type : default))
            .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Question != null ? src.Question.Points : 0))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Question != null ? src.Question.Options : null));

        // Assignment mappings
        CreateMap<Assignment, AssignmentDto>()
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
            .ForMember(dest => dest.LectureTitle, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : null))
            .ForMember(dest => dest.SubmissionCount, opt => opt.MapFrom(src => src.Submissions.Count));

        CreateMap<AssignmentSubmission, AssignmentSubmissionDto>()
            .ForMember(dest => dest.AssignmentTitle, opt => opt.MapFrom(src => src.Assignment != null ? src.Assignment.Title : null))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : null))
            .ForMember(dest => dest.MaxPoints, opt => opt.MapFrom(src => src.Assignment != null ? src.Assignment.MaxPoints : 0));

        // Notification mappings
        CreateMap<Notification, DTOs.Notifications.NotificationDto>();

        // Announcement mappings
        CreateMap<Announcement, DTOs.Announcements.AnnouncementDto>()
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : null));

        // AuditLog mappings
        CreateMap<AuditLog, DTOs.AuditLogs.AuditLogDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null));
    }
}
