using Backend.Application.Common;
using Backend.Application.DTOs.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Services
{
    public interface IPlanGenerator
    {
        Task<BaseResponse<PlanFullDto>> GeneratePlanAsync(GeneratePlanRequest request);
    }
}
