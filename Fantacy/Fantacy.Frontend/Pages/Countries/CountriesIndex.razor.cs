using Fantacy.Frontend.Repositories;
using Fantacy.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace Fantacy.Frontend.Pages.Countries;

public partial class CountriesIndex
{
    [Inject] private IRepository Repository { get; set; } = default!;

    private List<Country>? Countries { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await Repository.GetAsync<List<Country>>("api/countries");

        Countries = response.Response;
    }
}