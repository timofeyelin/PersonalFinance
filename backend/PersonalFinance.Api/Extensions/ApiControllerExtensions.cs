using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PersonalFinance.Api.Extensions;

public static class ApiControllerExtensions
{
    public static IActionResult Problem(this ControllerBase controller, List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return controller.Problem();
        }
        
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var modelStateDictionary = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }
            return controller.ValidationProblem(modelStateDictionary);
        }
        
        var firstError = errors[0];
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.Problem(
            statusCode: statusCode, 
            title: firstError.Description, 
            detail: firstError.Code);
    }
}