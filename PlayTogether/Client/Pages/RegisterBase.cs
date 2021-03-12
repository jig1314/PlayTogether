﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.Models;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;

namespace PlayTogether.Client.Pages
{
    public class RegisterBase : ComponentBase
    {
        [Parameter]
        public string ReturnUrl { get; set; }

        [Inject]
        public IUnauthorizedUserService UserService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public List<Gender> Genders { get; set; }

        public List<Country> Countries { get; set; }

        public RegisterViewModel RegisterViewModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            RegisterViewModel = new RegisterViewModel();
            Genders = await UserService.GetGenders();
            Countries = await UserService.GetCountries();
        }

        protected async Task RegisterUser()
        {
            var registerUserDto = new RegisterUserDto()
            {
                FirstName = RegisterViewModel.FirstName,
                LastName = RegisterViewModel.LastName,
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.UserName,
                Password = RegisterViewModel.Password,
                GenderId = RegisterViewModel.GenderId.Value,
                CountryOfResidenceId = RegisterViewModel.CountryOfResidenceId.Value,
                DateOfBirth = RegisterViewModel.DateOfBirth.Value
            };

            await UserService.RegisterNewUser(registerUserDto);
            NavigationManager.NavigateTo("authentication/login");

        }
    }
}
