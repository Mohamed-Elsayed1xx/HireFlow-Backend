using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyHandler : IRequestHandler<RegisterCompanyCommand, Result<LoginResponse>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public RegisterCompanyHandler(
        ICompanyRepository companyRepository,
        IPlanRepository planRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _companyRepository = companyRepository;
        _planRepository = planRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(
        RegisterCompanyCommand request,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(request.AdminEmail))
            return Result<LoginResponse>.Fail("EMAIL_EXISTS", "Email is already in use.");

        var slug = request.CompanyName.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Trim();

        if (await _companyRepository.SlugExistsAsync(slug))
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";

        var defaultPlan = (await _planRepository.GetActiveAsync())
            .OrderBy(p => p.Price)
            .FirstOrDefault();

        if (defaultPlan is null)
            return Result<LoginResponse>.Fail("NO_PLAN_AVAILABLE", "No subscription plan is currently available for new companies.");

        var company = new Company
        {
            Name = request.CompanyName,
            Slug = slug,
            Industry = request.Industry,
            Size = request.Size,
            PlanId = defaultPlan.Id,
            IsActive = true
        };

        await _companyRepository.AddAsync(company);

        var admin = new User
        {
            CompanyId = company.Id,
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            Email = request.AdminEmail,
            PasswordHash = _passwordService.Hash(request.AdminPassword),
            Role = UserRole.CompanyAdmin,
            IsActive = true
        };

        await _userRepository.AddAsync(admin);

        var accessToken = _tokenService.GenerateAccessToken(admin.Id, admin.Email, admin.Role.ToString(), admin.CompanyId);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshTokenEntity
        {
            UserId = admin.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return Result<LoginResponse>.Ok(new LoginResponse(
            accessToken,
            refreshToken,
            admin.Email,
            admin.Role.ToString()
        ));
    }
}