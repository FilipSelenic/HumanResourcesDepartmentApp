using AutoMapper;
using HumanResourcesDepartment.Models.DTO;
using HumanResourcesDepartment.Models;
using HumanResourcesDepartment.Repository.Interfaces;
using System.Net;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using HumanResourcesDepartment.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Microsoft.EntityFrameworkCore;

namespace HumanResourcesDepartment.Services
{
    public class EmployeesService
    {
        private readonly IMapper _mapper;
        private readonly IEmployeesRepository _employeeRepository;
        private readonly IValidator<EmployeeCreateDTO> _employeeCreateValidator;
        private readonly IValidator<EmployeeUpdateDTO> _employeeUpdateValidator;

        public EmployeesService(IMapper mapper, IValidator<EmployeeUpdateDTO> employeeUpdateValidator, IEmployeesRepository employeeRepository, IValidator<EmployeeCreateDTO> employeeCreateValidator)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _employeeCreateValidator = employeeCreateValidator;
            _employeeUpdateValidator = employeeUpdateValidator;
        }

        public IResult GetAllEmployees()
        {
            var employees = _employeeRepository.GetAll();
            var employeeDTOs = employees.ProjectTo<EmployeeDTO>(_mapper.ConfigurationProvider).ToList();

            var response = new APIResponse
            {
                Result = employeeDTOs,
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK
            };

            return Results.Ok(response);
        }
        public IResult GetEmployeeById(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                var response = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound
                };

                return Results.NotFound(response);
            }

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);

            var successResponse = new APIResponse
            {
                Result = employeeDTO,
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK
            };

            return Results.Ok(successResponse);
        }
        public IResult CreateEmployee(EmployeeCreateDTO employeeCreateDTO)
        {
            var validationResult = _employeeCreateValidator.ValidateAsync(employeeCreateDTO).GetAwaiter().GetResult();
            if (!validationResult.IsValid)
            {
                var response = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };

                return Results.BadRequest(response);
            }

            var employee = _mapper.Map<Employee>(employeeCreateDTO);
            _employeeRepository.Add(employee);

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);

            var successResponse = new APIResponse
            {
                Result = employeeDTO,
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created
            };

            return Results.Ok(successResponse);
        }
        public IResult UpdateEmployee(EmployeeUpdateDTO employeeUpdateDTO)
        {
            var validationResult = _employeeUpdateValidator.ValidateAsync(employeeUpdateDTO).GetAwaiter().GetResult();
            if (!validationResult.IsValid)
            {
                var response = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };

                return Results.BadRequest(response);
            }

            var employee = _mapper.Map<Employee>(employeeUpdateDTO);

            try
            {
                _employeeRepository.Update(employee);
            }
            catch (DbUpdateConcurrencyException e)
            {
                var response = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { e.Message }
                };

                return Results.BadRequest(response);
            }
            catch (Exception e)
            {
                var response = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { e.Message }
                };

                return Results.BadRequest(response);
            }

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);

            var successResponse = new APIResponse
            {
                Result = employeeDTO,
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK
            };

            return Results.Ok(successResponse);
        }
        public IResult DeleteEmployee(int id)
        {
            var existingEmployee = _employeeRepository.GetById(id);
            if (existingEmployee == null)
            {
                var response = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "Invalid id" }
                };

                return Results.NotFound(response);
            }

            _employeeRepository.Delete(existingEmployee);

            var successResponse = new APIResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.NoContent
            };

            return Results.Ok(successResponse);
        }
    }
}
