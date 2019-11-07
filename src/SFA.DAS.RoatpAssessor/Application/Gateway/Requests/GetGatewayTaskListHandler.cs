using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Application.Extensions;
using SFA.DAS.RoatpAssessor.Services.ApplyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayTaskListHandler : IRequestHandler<GetGatewayTaskListRequest, GetGatewayTaskListResponse>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public GetGatewayTaskListHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<GetGatewayTaskListResponse> Handle(GetGatewayTaskListRequest request, CancellationToken cancellationToken)
        {
            var reviewTask = _applyApiClient.GetApplicationReviewAsync(request.ApplicationId);
            var qnaApplicationDataTask = _qnaApiClient.GetApplicationData(request.ApplicationId);
            var sequencesTask = _qnaApiClient.GetAllApplicationSequences(request.ApplicationId);

            await Task.WhenAll(reviewTask, qnaApplicationDataTask, sequencesTask);

            var review = reviewTask.Result;
            var applicationData = new RoatpAssessorApplicationData(qnaApplicationDataTask.Result);

            if (!review.GatewayReviewIsInProgress)
            {
                throw new Exception($"Review '{request.ApplicationId}' is not in correct state to do Gateway review");
            }

            var gatewaySequences = sequencesTask.Result.Where(s => 
                RoatpWorkflowSequenceIds.GatewaySequences.Contains(s.SequenceNo)).ToList();

            var response = new GetGatewayTaskListResponse
            {
                ApplicationId = review.ApplicationId,
                ApplicationRef = review.ApplicationRef,
                SubmittedAt = review.ApplicationSubmittedAt,
                CanDoLegalChecks = review.CanDoGatewayLegalChecks,
                CanDoAddressChecks = review.CanDoGatewayAddressChecks,
                Ukprn = applicationData.UKPRN,
                Name = applicationData.UKRLP_LegalName,
                ProviderRoute = applicationData.Apply_ProviderRoute.ToString()
            };

            return response;
        }
    }
}
