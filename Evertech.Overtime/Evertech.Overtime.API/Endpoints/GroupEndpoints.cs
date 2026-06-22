using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class GroupEndpoints
{
    public static IEndpointRouteBuilder AddGroupEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/groups").WithTags("Groups").RequireAuthorization();

        group.MapPost("/", async (
            CreateGroupModel model,
            IGroupService groupService,
            IValidator<CreateGroupModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await groupService.CreateAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IGroupService groupService,
            CancellationToken cancellationToken) =>
        {
            var result = await groupService.GetByIdAsync(id, cancellationToken);
            return result is null
                ? Results.NotFound()
                : Results.Ok(new { data = result });
        });

        group.MapGet("/active", async (
            IGroupService groupService,
            CancellationToken cancellationToken) =>
        {
            var result = await groupService.GetActiveAsync(cancellationToken);
            return Results.Ok(new { data = result });
        });

        group.MapPut("/", async (
            UpdateGroupModel model,
            IGroupService groupService,
            IValidator<UpdateGroupModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await groupService.UpdateAsync(model, cancellationToken);
            return Results.NoContent();
        });

        group.MapPost("/members", async (
            AddPersonToGroupModel model,
            IGroupService groupService,
            IValidator<AddPersonToGroupModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await groupService.AddMemberAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        group.MapDelete("/members", async (
            RemoveGroupMemberModel model,
            IGroupService groupService,
            IValidator<RemoveGroupMemberModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await groupService.RemoveMemberAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        group.MapPatch("/leaders", async (
            SetGroupLeaderModel model,
            IGroupService groupService,
            IValidator<SetGroupLeaderModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await groupService.SetLeaderAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        return builder;
    }
}
