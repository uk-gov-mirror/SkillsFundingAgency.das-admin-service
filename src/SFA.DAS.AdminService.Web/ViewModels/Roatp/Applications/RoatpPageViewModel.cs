using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Page = SFA.DAS.QnA.Api.Types.Page.Page;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpPageViewModel : OrganisationDetailsViewModel
    {
        public Page Page { get; }
        public string Title { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }
        public string PageId { get; }

        public string ReviewStatus { get; set; }
        public string ReviewComments { get; set; }

        public Dictionary<string, AddressViewModel> Addresses = new Dictionary<string, AddressViewModel>();

        public RoatpPageViewModel() { }

        public RoatpPageViewModel(RoatpApplicationResponse application, Section section, Page page)
        {
            ApplicationId = application.ApplicationId;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
            ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
            SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            Ukprn = application.ApplyData.ApplyDetails.UKPRN;

            Page = page;
            SequenceNo = section.SequenceNo;
            SectionNo = section.SectionNo;
            PageId = page.PageId;
            Title = page.Title;

            SetupReviewInformation(application.ApplyData);

            foreach (var answerPage in page.PageOfAnswers)
            {
                foreach (var answer in answerPage.Answers)
                {
                    var question = page.Questions.SingleOrDefault(q => q.QuestionId == answer.QuestionId);
                    if (question != null && question.Input.Type == "Address")
                    {
                        Addresses.Add(answer.QuestionId, JsonConvert.DeserializeObject<AddressViewModel>(answer.Value));
                    }
                }
            }
        }

        private void SetupReviewInformation(RoatpApplyData applyData)
        {
            var applySequence = applyData?.Sequences?.SingleOrDefault(seq => seq.SequenceNo == SequenceNo);
            var applySection = applySequence?.Sections?.SingleOrDefault(sec => sec.SectionNo == SectionNo);
            var applyPage = applySection?.Pages?.SingleOrDefault(pg => pg.PageId == PageId);

            if (applyPage != null)
            {
                ReviewStatus = applyPage.ReviewStatus;
                ReviewComments = applyPage.ReviewComments?.LastOrDefault()?.Comment;
            }
        }

        public string DisplayAnswerValue(QnA.Api.Types.Page.Answer answer, QnA.Api.Types.Page.Question question)
        {
            if (question?.Input?.Type == "Date" || question?.Input?.Type == "MonthAndYear")
            {
                var dateparts = answer.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (question.Input.Type == "Date")
                {
                    var datetime = DateTime.Parse($"{dateparts[0]}/{dateparts[1]}/{dateparts[2]}");
                    return datetime.ToString("dd/MM/yyyy");
                }
                else if (question.Input.Type == "MonthAndYear")
                {
                    DateTime datetime;
                    DateTime.TryParse($"{dateparts[0]}/{dateparts[1]}", out datetime);
                    return datetime.ToString("MM/yyyy");
                }
            }

            return answer.Value;
        }

        public bool QuestionContainsPredefinedOptions(QnA.Api.Types.Page.Question question)
        {
            if (question?.Input?.Type == null)
            {
                return false;
            }

            switch (question.Input.Type)
            {
                case "CheckBox":
                case "CheckBoxList":
                case "ComplexCheckBoxList":
                case "ComplexRadio":
                case "Radio":
                    return true;
                default:
                    return false;
            }
        }
    }
}
