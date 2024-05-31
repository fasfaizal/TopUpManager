using Microsoft.Extensions.Options;
using System.Net;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.Services.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepo _beneficiaryRepo;
        private readonly IUserRepo _userRepo;
        private readonly Configurations _configurations;

        public BeneficiaryService(IBeneficiaryRepo beneficiaryRepo, IUserRepo userRepo, IOptions<Configurations> config)
        {
            _beneficiaryRepo = beneficiaryRepo;
            _userRepo = userRepo;
            _configurations = config.Value;
        }

        /// <summary>
        /// Retrieves a list of beneficiaries for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user for whom to retrieve beneficiaries.
        /// </param>
        /// <returns>
        /// List of beneficiaries for a user.
        /// </returns>
        public async Task<List<Beneficiary>> GetBeneficiariesAsync(int userId)
        {
            return await _beneficiaryRepo.GetBeneficiariesAsync(userId);
        }

        /// <summary>
        /// Creates a new beneficiary for a user.
        /// </summary>
        /// <param name="beneficiaryRequest">The request model containing the details of the beneficiary to be created.</param>
        /// <returns>
        /// An object representing the newly created beneficiary.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="beneficiaryRequest"/> is null.</exception>
        /// <exception cref="ApiException">
        /// Thrown when the user does not exist (HTTP 400 Bad Request) or the beneficiary limit is reached (HTTP 422 Unpossessable Entity).
        /// </exception>
        public async Task<Beneficiary> CreateBeneficiaryAsync(BeneficiaryRequestModel beneficiaryRequest)
        {
            if (beneficiaryRequest == null)
            {
                throw new ArgumentNullException(nameof(beneficiaryRequest));
            }

            // Check if user exists
            var user = await _userRepo.GetUserByIdAsync(beneficiaryRequest.UserId);
            if (user == null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid user");
            }

            // Check if beneficiary count is greater than MaxBeneficiaryCount
            if (user.Beneficiaries.Count >= _configurations.MaxBeneficiaryCount)
            {
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "Beneficiary limit reached");
            }

            // Add new beneficiary
            var newBeneficiary = new Beneficiary
            {
                Nickname = beneficiaryRequest.Nickname,
                UserId = beneficiaryRequest.UserId
            };
            await _beneficiaryRepo.AddBeneficiaryAsync(newBeneficiary);
            return newBeneficiary;
        }
    }
}
