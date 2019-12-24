using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    [Authorize]
    public class AssessmentSpikeController : Controller
    {
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApplyApiClient _roatpApplyApiClient;

        public AssessmentSpikeController(IQnaApiClient qnaApiClient, IRoatpApplyApiClient roatpApplyApiClient)
        {
            _qnaApiClient = qnaApiClient;
            _roatpApplyApiClient = roatpApplyApiClient;
        }

        [Route("assessment-spike")]
        public async Task<IActionResult> Index(Guid applicationId)
        {
            var config = GetConfig(applicationId);

            var model = await BuildViewModelFromConfiguration(config);

            return View("~/Views/RoatpAssessor/AssessmentSpike.cshtml", model);
        }

        public async Task<IActionResult> Download(Guid Id, int sequenceNo, int sectionId, string pageId, string questionId, string filename)
        {
            var sequences = await _qnaApiClient.GetAllApplicationSequences(Id);
            var selectedSequence = sequences.Single(x => x.SequenceNo == sequenceNo);
            var sections = await _qnaApiClient.GetSections(Id, selectedSequence.Id);
            var selectedSection = sections.Single(x => x.SectionNo == sectionId);
            var response = await _qnaApiClient.DownloadFile(Id, selectedSection.Id, pageId, questionId, filename);
            var fileStream = await response.Content.ReadAsStreamAsync();

            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);

        }

        private AssessmentQuestionConfiguration GetConfig(Guid applicationId)
        {
            var config = new AssessmentQuestionConfiguration { ApplicationId = applicationId };

            config.SectionId = 2;
            config.SequenceId = 4;
            config.SectionTitle = "Protecting your apprentices checks";
            config.Heading = "Check the organisation's equality and diversity policy";

            config.AssessmentQuestions = new List<AssessmentQuestion>
            {
                //new AssessmentQuestion
                //{
                //    PageId = "4035",
                //    QuestionId = "PYA-45"
                //},
                //new AssessmentQuestion
                //{
                //    PageId = "4035",
                //    QuestionId = "PYA-46"
                //},
                //new AssessmentQuestion
                //{
                //    PageId = "4035",
                //    QuestionId = "PYA-47"
                //},
                new AssessmentQuestion
                {
                    PageId = "4010",
                    QuestionId = "PYA-20"
                }
            };

            config.AssessmentCriteria = new List<AssessmentCriteria>()
            {
                new AssessmentCriteria
                {
                    AssessmentQuestion = "Does the upload include how the organisation will promote the policy?"
                },
                new AssessmentCriteria
                {
                    AssessmentQuestion = "Does the upload include how the organisation will get engagement towards the policy?"
                },
                new AssessmentCriteria
                {
                    AssessmentQuestion = "Does the upload include how the organisation will train its employees in implementing the policy?"
                },
                new AssessmentCriteria
                {
                    AssessmentQuestion = "Does the upload include how the organisation will consider the policy when recruiting, delivering apprenticeship training and working with employers and apprentices?"
                },
                new AssessmentCriteria
                {
                    AssessmentQuestion = "Is the upload specific to apprentices, trainers and the applying organisation?"
                },
                new AssessmentCriteria
                {
                    AssessmentQuestion = "Is the upload signed by a senior employee?",
                    HelpText = "For example, the upload is signed by a director or CEO."
                }                
            };
            config.AssessmentSOPs = "<p class='govuk-body'>If you have selected 'yes' to all of the questions above, pass this check.</p> <p class='govuk-body'>If you have selected 'no' to any of the questions above, fail this check.</p>";

            var json = JsonConvert.SerializeObject(config);

            return config;
        }

        private async Task<AssessmentViewModel> BuildViewModelFromConfiguration(AssessmentQuestionConfiguration config)
        {
            var applications = await _roatpApplyApiClient.GetSubmittedApplications();
            var submittedApplication = applications.FirstOrDefault(x => x.ApplicationId == config.ApplicationId);

            var applicationData = await _qnaApiClient.GetApplicationData(config.ApplicationId);

            var model = new AssessmentViewModel
            {
                ApplicationId = config.ApplicationId,
                SectionId = config.SectionId,
                SequenceId = config.SequenceId,
                SectionTitle = config.SectionTitle,
                Heading = config.Heading,
                AssessmentCriteria = config.AssessmentCriteria,
                Questions = new List<AssessmentQuestion>(),
                UKPRN = applicationData["UKPRN"].ToString(),
                LegalName = applicationData["UKRLP-LegalName"].ToString(),
                AssessmentSOPs = config.AssessmentSOPs
            };

            foreach(var question in config.AssessmentQuestions)
            {
                var page = await _qnaApiClient.GetPageBySectionNo(config.ApplicationId, config.SequenceId, config.SectionId, question.PageId);
                var qnaQuestion = page.Questions.FirstOrDefault(x => x.QuestionId == question.QuestionId);

                var assessmentQuestion = new AssessmentQuestion();
                assessmentQuestion.QuestionId = question.QuestionId;
                assessmentQuestion.QuestionText = qnaQuestion.Label;
                assessmentQuestion.QuestionType = qnaQuestion.Input.Type;
                assessmentQuestion.PageId = question.PageId;
                foreach (var pageOfAnswers in page.PageOfAnswers)
                {
                    var matchedAnswer = pageOfAnswers.Answers.FirstOrDefault(y => y.QuestionId == question.QuestionId);
                    if (matchedAnswer != null)
                    {
                        assessmentQuestion.Answer = matchedAnswer.Value;
                    }
                }
                model.Questions.Add(assessmentQuestion);
            }

            return await Task.FromResult(model);
        }
    }

    public class AssessmentViewModel
    {
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public int SectionId { get; set; }

        public string SectionTitle { get; set; }
        public string Heading { get; set; }

        public string LegalName { get; set; }
        public string UKPRN { get; set; }

        public List<AssessmentQuestion> Questions { get; set; }
        public List<AssessmentCriteria> AssessmentCriteria { get; set; }

        public string AssessmentSOPs { get; set; }

        public AssessmentOutcome AssessmentOutcome { get; set; }
    }

    public class AssessmentQuestion
    {
        public string PageId { get; set; }      
        public string QuestionId { get; set; }
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
    }

    public class AssessmentCriteria
    {
        public string AssessmentQuestion { get; set; }
        public string HelpText { get; set; }
        public bool CriteriaMet { get; set; }
    }

    public enum AssessmentOutcome
    {
        Pass,
        Fail
    }

    // stored in database in JSON

    public class AssessmentQuestionConfiguration
    {
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public int SectionId { get; set; }
        public string SectionTitle { get; set; }
        public string Heading { get; set; }
        public List<AssessmentQuestion> AssessmentQuestions { get; set; }
        public List<AssessmentCriteria> AssessmentCriteria { get; set; }
        public string AssessmentSOPs { get; set; }
    }
}
