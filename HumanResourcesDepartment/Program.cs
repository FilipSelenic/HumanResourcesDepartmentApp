using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using FluentValidation;
using HumanResourcesDepartment;
using HumanResourcesDepartment.Models;
using HumanResourcesDepartment.Models.DTO;
using HumanResourcesDepartment.Repository;
using HumanResourcesDepartment.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("AppConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var Configuration = builder.Configuration.GetSection("Jwt");

// Adding Authentication  
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer  
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = Configuration["Audience"],
        ValidIssuer = Configuration["Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Key"]))
    };
});


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();




app.MapPost("/login",  (UserManager<ApplicationUser> userManager, IValidator<LoginDTO> _validation, [FromBody] LoginDTO loginDTO) =>
{
    var validationResult = _validation.ValidateAsync(loginDTO).GetAwaiter().GetResult();
    if (!validationResult.IsValid)
    {
        return Results.BadRequest();
    }

    var user = userManager.FindByNameAsync(loginDTO.Username).GetAwaiter().GetResult();
    if (user != null && userManager.CheckPasswordAsync(user, loginDTO.Password).GetAwaiter().GetResult())
    {

        var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Key"]));

        var token = new JwtSecurityToken(
            issuer: Configuration["Issuer"],
            audience: Configuration["Audience"],
            expires: DateTime.Now.AddHours(2),      // token valid for 2 hours
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return Results.Ok(new TokenDTO()
        {
            Username = user.UserName,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        });
    }
    return Results.Unauthorized();
});


app.MapPost("/register", (UserManager<ApplicationUser> userManager, IValidator<RegistrationDTO> _validation, [FromBody] RegistrationDTO registrationDTO) => {

    var validationResult = _validation.ValidateAsync(registrationDTO).GetAwaiter().GetResult();
    if (!validationResult.IsValid)
    {
        return Results.BadRequest();
    }

    var userExists = userManager.FindByNameAsync(registrationDTO.Username).GetAwaiter().GetResult();
    if (userExists != null)
    {
        return Results.BadRequest("User already exists");
    }

    ApplicationUser user = new ApplicationUser()
    {
        Email = registrationDTO.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = registrationDTO.Username
    };
    var result = userManager.CreateAsync(user, registrationDTO.Password).GetAwaiter().GetResult();
    if (!result.Succeeded)
    {
        return Results.BadRequest("Validation failed! Please check user details and try again.");
    }

    return Results.Ok();
});




app.MapGet("/api/employees", (IMapper _mapper, IEmployeesRepository employeeRepository) =>
{
    var employees = employeeRepository.GetAll();
    APIResponse response = new();
    response.Result = employees.ProjectTo<EmployeeDTO>(_mapper.ConfigurationProvider).ToList();
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetEmployees").Produces<APIResponse>(200);

app.MapGet("/api/employees/{id:int}", (IMapper _mapper, IEmployeesRepository employeeRepository, int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

    var employee = employeeRepository.GetById(id);
    if (employee == null)
    {
        return Results.NotFound(response);
    }
    response.Result = _mapper.Map<EmployeeDTO>(employee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetEmployee").Produces<APIResponse>(200).Produces(404);

app.MapPost("/api/employees",  (IMapper _mapper, IValidator< EmployeeCreateDTO> _validation, IEmployeesRepository employeeRepository, [FromBody] EmployeeCreateDTO employeeCreateDTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult =  _validation.ValidateAsync(employeeCreateDTO).GetAwaiter().GetResult();
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
  
    Employee employee = _mapper.Map<Employee>(employeeCreateDTO);
    employeeRepository.Add(employee);
    response.Result = _mapper.Map<EmployeeDTO>(employee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.Ok(response);
}).WithName("CreateEmployee").Accepts<EmployeeCreateDTO>("application/json").Produces<APIResponse>(201);

app.MapPut("/api/employees", (IMapper _mapper, IValidator<EmployeeUpdateDTO> _validation, IEmployeesRepository employeeRepository, [FromBody] EmployeeUpdateDTO employeeUpdateDTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = _validation.ValidateAsync(employeeUpdateDTO).GetAwaiter().GetResult();

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    Employee employee = _mapper.Map<Employee>(employeeUpdateDTO);

    try
    {
        employeeRepository.Update(employee);
    }
    catch(DbUpdateConcurrencyException e)
    {
        response.ErrorMessages.Add(e.Message);
        return Results.BadRequest(response);
    }
    catch (Exception e)
    {
        response.ErrorMessages.Add(e.Message);
        return Results.BadRequest(response);
    }
    response.Result = _mapper.Map<EmployeeDTO>(employee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK; 
    return Results.Ok(response);
}).Produces<APIResponse>(200).Produces(400).Accepts<EmployeeUpdateDTO>("application/json");

app.MapDelete("/api/employees/{id:int}", (IEmployeesRepository employeeRepository, int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

    var existingEmployee = employeeRepository.GetById(id);
    if (existingEmployee == null)
    {
        response.ErrorMessages.Add("Invalid id");
        return Results.NotFound(response);
    }

    employeeRepository.Delete(existingEmployee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);
}).Produces(200).Produces(404);


app.Run();


