using ContractManager.Communication.Dtos.Endpoints.Update;
using FluentValidation;

namespace ContactManager.Api.Validators
{
    public class UpdateRequestValidator : AbstractValidator<UpdateRequest>
    {
        public UpdateRequestValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(512);
            RuleFor(x => x.Phone).MaximumLength(512).When(x => x.Phone != null);
        }
    }
}
