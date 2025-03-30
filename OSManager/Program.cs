using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OSManager.Services;
using OSManager.Data;
using OSManager.DTOs;
using OSManager.Middleware;
using OSManager.Models;
using OSSystem.Services;
using OSManager.Utils;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Adicionar serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Ordem de Serviço API",
        Version = "v1",
        Description = "API para gerenciamento de ordens de serviço, checklist e evidências fotográficas",
        Contact = new OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@exemplo.com",
            Url = new Uri("https://exemplo.com/suporte")
        },
        License = new OpenApiLicense
        {
            Name = "Uso Interno",
            Url = new Uri("https://exemplo.com/licenca")
        }
    });

    // Agrupar endpoints por controlador
    c.TagActionsBy(api => [api.HttpMethod]);

    // Ordenar ações
    c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

    // Configuração para autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o scheme Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Adicionar comentários XML se estiverem disponíveis
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                ["http://localhost:8080", "https://localhost:8081"]
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configurar banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configurar autenticação JWT
var jwtKey = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment12345678");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "OSManager";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "OSManager";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Elimina tolerância de tempo
        };
    });

builder.Services.AddAuthorization();

// Registrar serviços
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ChecklistService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<EmailService>();

// Configurar política de autorização
builder.Services.AddAuthorizationBuilder()
                                         // Configurar política de autorização
                                         .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
                                         // Configurar política de autorização
                                         .AddPolicy("RequireSupervisorRole", policy => policy.RequireRole("Admin", "Supervisor"));

var app = builder.Build();

// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Aplicar migrações
        dbContext.Database.Migrate();

        // Inicializar dados
        await DbInitializer.InitializeAsync(dbContext);

        logger.LogInformation("Banco de dados migrado e inicializado com sucesso.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro ao migrar ou inicializar o banco de dados.");
    }
}

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Configurar pasta de uploads para acesso estático
app.UseStaticFiles();

// Endpoints

// Autenticação
app.MapPost("/api/auth/login", async (LoginRequest request, AuthService authService) =>
{
    var result = await authService.AuthenticateAsync(request.Username, request.Password);

    if (result == null)
        return Results.Unauthorized();

    return Results.Ok(new LoginResponse
    {
        Token = result.Value.token,
        Username = result.Value.user.Username
    });
})
.WithName("Login")
.WithOpenApi();

// Gestão de Usuários
app.MapGet("/api/users", async (UserService userService) =>
{
    var users = await userService.GetUsersAsync();

    return Results.Ok(users.Select(u => new UserDto
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        FullName = u.FullName,
        Role = u.Role,
        IsActive = u.IsActive
    }));
})
.RequireAuthorization("RequireAdminRole")
.WithName("GetUsers")
.WithOpenApi();

app.MapGet("/api/users/{id}", async (int id, UserService userService) =>
{
    var user = await userService.GetUserByIdAsync(id);

    if (user is null)
        return Results.NotFound();

    return Results.Ok(new UserDto
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        IsActive = user.IsActive
    });
})
.RequireAuthorization("RequireAdminRole")
.WithName("GetUserById")
.WithOpenApi();

app.MapPost("/api/users", async (RegisterUserRequest request, UserService userService) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) ||
        string.IsNullOrWhiteSpace(request.Password) ||
        string.IsNullOrWhiteSpace(request.Email))
        return Results.BadRequest("Campos obrigatórios não preenchidos");

    var existingUser = await userService.GetUserByUsernameAsync(request.Username);
    if (existingUser is not null)
        return Results.BadRequest("Nome de usuário já existe");

    var existingEmail = await userService.GetUserByEmailAsync(request.Email);
    if (existingEmail is not null)
        return Results.BadRequest("Email já cadastrado");

    var user = await userService.RegisterUserAsync(
        request.Username,
        request.Password,
        request.Email,
        request.FullName,
        request.Role
    );

    return Results.Created($"/api/users/{user.Id}", new UserDto
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        IsActive = user.IsActive
    });
})
.RequireAuthorization("RequireAdminRole")
.WithName("RegisterUser")
.WithOpenApi();

app.MapPut("/api/users/{id}", async (int id, UpdateUserRequest request, UserService userService) =>
{
    if (id != request.Id)
        return Results.BadRequest("ID inconsistente");

    var result = await userService.UpdateUserAsync(
        id,
        request.Email,
        request.FullName,
        request.Role,
        request.IsActive
    );

    if (!result)
        return Results.NotFound();

    return Results.NoContent();
})
.RequireAuthorization("RequireAdminRole")
.WithName("UpdateUser")
.WithOpenApi();

app.MapPost("/api/users/change-password", async (ChangePasswordRequest request, ClaimsPrincipal user, UserService userService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var result = await userService.ChangePasswordAsync(
        userId,
        request.CurrentPassword,
        request.NewPassword
    );

    if (!result)
        return Results.BadRequest("Senha atual incorreta");

    return Results.Ok();
})
.RequireAuthorization()
.WithName("ChangePassword")
.WithOpenApi();

app.MapPost("/api/auth/forgot-password", async (ForgotPasswordRequest request, UserService userService) =>
{
    var token = await userService.GeneratePasswordResetTokenAsync(request.Email);

    if (token == null)
        return Results.Ok(); // Não revelar se o email existe

    // Aqui você enviaria um email com o token
    // Por enquanto, apenas retornamos o token para testes
    return Results.Ok(new { token });
})
.WithName("ForgotPassword")
.WithOpenApi();

app.MapPost("/api/auth/reset-password", async (ResetPasswordRequest request, UserService userService) =>
{
    var result = await userService.ResetPasswordAsync(
        request.Token,
        request.NewPassword
    );

    if (!result)
        return Results.BadRequest("Token inválido ou expirado");

    return Results.Ok();
})
.WithName("ResetPassword")
.WithOpenApi();

// Ordens de Serviço
app.MapGet("/api/orders", async (ClaimsPrincipal user, OrderService orderService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    List<Order> orders;

    if (user.IsInRole("Admin") || user.IsInRole("Supervisor"))
    {
        orders = await orderService.GetAllOrdersAsync();
    }
    else
    {
        orders = await orderService.GetOrdersAsync(userId);
    }

    return Results.Ok(orders.Select(o => new OrderResponse
    {
        Id = o.Id,
        Description = o.Description,
        CreatedAt = o.CreatedAt,
        StartedAt = o.StartedAt,
        CompletedAt = o.CompletedAt,
        ApprovedAt = o.ApprovedAt,
        Status = o.Status,
        RejectionReason = o.RejectionReason,
        User = new UserDto
        {
            Id = o.User?.Id ?? 0,
            Username = o.User?.Username ?? "",
            FullName = o.User?.FullName ?? ""
        },
        Approver = o.Approver == null ? null : new UserDto
        {
            Id = o.Approver.Id,
            Username = o.Approver.Username,
            FullName = o.Approver.FullName
        },
        ChecklistItems = [.. o.ChecklistItems.Select(c => new ChecklistItemDto
        {
            Id = c.Id,
            Description = c.Description,
            IsCompleted = c.IsCompleted,
            Category = c.Category,
            DisplayOrder = c.DisplayOrder
        })],
        Images = [.. o.Images.Select(i => new ImageDto
        {
            Id = i.Id,
            FileName = i.FileName,
            ContentType = i.ContentType,
            FileSize = i.FileSize,
            UploadedAt = i.UploadedAt
        })],
        AllChecklistItemsCompleted = o.ChecklistItems.All(c => c.IsCompleted)
    }));
})
.RequireAuthorization()
.WithName("GetOrders")
.WithOpenApi();

app.MapGet("/api/orders/filter", async (string? status, DateTime? fromDate, DateTime? toDate, int? userId, ClaimsPrincipal user, OrderService orderService) =>
{
    var currentUserId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Se não for admin/supervisor, só pode ver suas próprias ordens
    if (!user.IsInRole("Admin") && !user.IsInRole("Supervisor"))
    {
        userId = currentUserId;
    }

    var orders = await orderService.GetFilteredOrdersAsync(userId, status, fromDate, toDate);

    return Results.Ok(orders.Select(o => new OrderResponse
    {
        Id = o.Id,
        Description = o.Description,
        CreatedAt = o.CreatedAt,
        StartedAt = o.StartedAt,
        CompletedAt = o.CompletedAt,
        ApprovedAt = o.ApprovedAt,
        Status = o.Status,
        RejectionReason = o.RejectionReason,
        User = new UserDto
        {
            Id = o.User?.Id ?? 0,
            Username = o.User?.Username ?? "",
            FullName = o.User?.FullName ?? ""
        },
        Approver = o.Approver == null ? null : new UserDto
        {
            Id = o.Approver.Id,
            Username = o.Approver.Username,
            FullName = o.Approver.FullName
        },
        ChecklistItems = [.. o.ChecklistItems.Select(c => new ChecklistItemDto
        {
            Id = c.Id,
            Description = c.Description,
            IsCompleted = c.IsCompleted,
            Category = c.Category,
            DisplayOrder = c.DisplayOrder
        })],
        Images = [.. o.Images.Select(i => new ImageDto
        {
            Id = i.Id,
            FileName = i.FileName,
            ContentType = i.ContentType,
            FileSize = i.FileSize,
            UploadedAt = i.UploadedAt
        })],
        AllChecklistItemsCompleted = o.ChecklistItems.All(c => c.IsCompleted)
    }));
})
.RequireAuthorization()
.WithName("FilterOrders")
.WithOpenApi();

app.MapGet("/api/orders/stats", async (ClaimsPrincipal user, OrderService orderService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Admins e Supervisores podem ver estatísticas gerais
    var statsUserId = (user.IsInRole("Admin") || user.IsInRole("Supervisor")) ? null : (int?)userId;

    var stats = await orderService.GetOrderStatisticsAsync(statsUserId);

    return Results.Ok(stats);
})
.RequireAuthorization()
.WithName("GetOrderStats")
.WithOpenApi();

app.MapGet("/api/orders/{id}", async (int id, ClaimsPrincipal user, OrderService orderService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var order = await orderService.GetOrderByIdAsync(id);

    if (order == null)
        return Results.NotFound();

    // Se não for admin/supervisor, só pode ver suas próprias ordens
    if (order.UserId != userId && !user.IsInRole("Admin") && !user.IsInRole("Supervisor"))
        return Results.Forbid();

    return Results.Ok(new OrderResponse
    {
        Id = order.Id,
        Description = order.Description,
        CreatedAt = order.CreatedAt,
        StartedAt = order.StartedAt,
        CompletedAt = order.CompletedAt,
        ApprovedAt = order.ApprovedAt,
        Status = order.Status,
        RejectionReason = order.RejectionReason,
        User = new UserDto
        {
            Id = order.User?.Id ?? 0,
            Username = order.User?.Username ?? "",
            FullName = order.User?.FullName ?? ""
        },
        Approver = order.Approver == null ? null : new UserDto
        {
            Id = order.Approver.Id,
            Username = order.Approver.Username,
            FullName = order.Approver.FullName
        },
        ChecklistItems = [.. order.ChecklistItems.Select(c => new ChecklistItemDto
        {
            Id = c.Id,
            Description = c.Description,
            IsCompleted = c.IsCompleted,
            Category = c.Category,
            DisplayOrder = c.DisplayOrder,
            Images = [.. c.Images.Select(i => new ImageDto
            {
                Id = i.Id,
                FileName = i.FileName,
                ContentType = i.ContentType,
                FileSize = i.FileSize,
                UploadedAt = i.UploadedAt
            })]
        })],
        Images = [.. order.Images.Where(i => i.ChecklistItemId == null).Select(i => new ImageDto
        {
            Id = i.Id,
            FileName = i.FileName,
            ContentType = i.ContentType,
            FileSize = i.FileSize,
            UploadedAt = i.UploadedAt
        })],
        AllChecklistItemsCompleted = order.ChecklistItems.All(c => c.IsCompleted)
    });
})
.RequireAuthorization()
.WithName("GetOrderById")
.WithOpenApi();

app.MapPost("/api/orders", async (CreateOrderRequest request, ClaimsPrincipal user, OrderService orderService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    if (string.IsNullOrWhiteSpace(request.Description))
        return Results.BadRequest("Descrição é obrigatória");

    var order = await orderService.CreateOrderAsync(request.Description, userId);

    return Results.Created($"/api/orders/{order.Id}", new OrderResponse
    {
        Id = order.Id,
        Description = order.Description,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        User = new UserDto
        {
            Id = order.User?.Id ?? 0,
            Username = order.User?.Username ?? "",
            FullName = order.User?.FullName ?? ""
        },
        ChecklistItems = [.. order.ChecklistItems.Select(c => new ChecklistItemDto
        {
            Id = c.Id,
            Description = c.Description,
            IsCompleted = c.IsCompleted,
            Category = c.Category,
            DisplayOrder = c.DisplayOrder
        })],
        AllChecklistItemsCompleted = false
    });
})
.RequireAuthorization()
.WithName("CreateOrder")
.WithOpenApi();

app.MapPut("/api/orders/status", async (UpdateOrderStatusRequest request, ClaimsPrincipal user, OrderService orderService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var order = await orderService.GetOrderByIdAsync(request.Id);

    if (order == null)
        return Results.NotFound();

    // Verificar permissões por status
    var canUpdateStatus = true;

    // Apenas admin/supervisor podem aprovar/rejeitar
    if ((request.Status == "Approved" || request.Status == "Rejected") &&
        !user.IsInRole("Admin") && !user.IsInRole("Supervisor"))
    {
        canUpdateStatus = false;
    }

    // Técnicos só podem atualizar suas próprias ordens
    if (!user.IsInRole("Admin") && !user.IsInRole("Supervisor") && order.UserId != userId)
    {
        canUpdateStatus = false;
    }

    if (!canUpdateStatus)
        return Results.Forbid();

    var result = await orderService.UpdateOrderStatusAsync(
        request.Id,
        request.Status,
        request.RejectionReason,
        (request.Status == "Approved" || request.Status == "Rejected") ? userId : null
    );

    if (!result)
        return Results.BadRequest("Não foi possível atualizar o status. Verifique se todos os itens do checklist estão concluídos para marcar como 'Completed'.");

    return Results.Ok();
})
.RequireAuthorization()
.WithName("UpdateOrderStatus")
.WithOpenApi();

// Checklist Items
app.MapPut("/api/checklist/items", async (UpdateChecklistItemRequest request, ClaimsPrincipal user, OrderService orderService, AppDbContext _context) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Verificar se o item pertence a uma ordem do usuário
    var item = await _context.ChecklistItems
        .Include(c => c.Order)
        .FirstOrDefaultAsync(c => c.Id == request.Id);

    if (item == null)
        return Results.NotFound();

    if (item.Order == null)
        return Results.BadRequest("Item não pertence a uma ordem");

    // Somente o técnico responsável ou admin/supervisor podem atualizar
    if (item.Order.UserId != userId && !user.IsInRole("Admin") && !user.IsInRole("Supervisor"))
        return Results.Forbid();

    // Não permitir atualizar itens de uma OS já aprovada/rejeitada
    if (item.Order.Status == "Approved" || item.Order.Status == "Rejected")
        return Results.BadRequest("Não é possível atualizar itens de uma OS já finalizada");

    var result = await orderService.UpdateChecklistItemAsync(request.Id, request.IsCompleted);

    if (!result)
        return Results.NotFound();

    return Results.Ok();
})
.RequireAuthorization()
.WithName("UpdateChecklistItem")
.WithOpenApi();

// Templates de Checklist
app.MapGet("/api/checklist/templates", async (ChecklistService checklistService) =>
{
    var templates = await checklistService.GetTemplatesAsync();

    return Results.Ok(templates.Select(t => new ChecklistTemplateDto
    {
        Id = t.Id,
        Description = t.Description,
        Category = t.Category,
        DisplayOrder = t.DisplayOrder
    }));
})
.WithName("GetChecklistTemplates")
.WithOpenApi();

app.MapGet("/api/checklist/templates/categories", async (ChecklistService checklistService) =>
{
    var categories = await checklistService.GetCategoriesAsync();

    return Results.Ok(categories);
})
.WithName("GetChecklistCategories")
.WithOpenApi();

app.MapGet("/api/checklist/templates/{id}", async (int id, ChecklistService checklistService) =>
{
    var template = await checklistService.GetTemplateByIdAsync(id);

    if (template == null)
        return Results.NotFound();

    return Results.Ok(new ChecklistTemplateDto
    {
        Id = template.Id,
        Description = template.Description,
        Category = template.Category,
        DisplayOrder = template.DisplayOrder
    });
})
.WithName("GetChecklistTemplateById")
.WithOpenApi();

app.MapPost("/api/checklist/templates", async (CreateChecklistTemplateRequest request, ChecklistService checklistService) =>
{
    if (string.IsNullOrWhiteSpace(request.Description))
        return Results.BadRequest("Descrição é obrigatória");

    var template = await checklistService.CreateTemplateAsync(
        request.Description,
        request.Category,
        request.DisplayOrder
    );

    return Results.Created($"/api/checklist/templates/{template.Id}", new ChecklistTemplateDto
    {
        Id = template.Id,
        Description = template.Description,
        Category = template.Category,
        DisplayOrder = template.DisplayOrder
    });
})
.RequireAuthorization("RequireAdminRole")
.WithName("CreateChecklistTemplate")
.WithOpenApi();

app.MapPut("/api/checklist/templates/{id}", async (int id, UpdateChecklistTemplateRequest request, ChecklistService checklistService) =>
{
    if (id != request.Id)
        return Results.BadRequest("ID inconsistente");

    if (string.IsNullOrWhiteSpace(request.Description))
        return Results.BadRequest("Descrição é obrigatória");

    var result = await checklistService.UpdateTemplateAsync(
        id,
        request.Description,
        request.Category,
        request.DisplayOrder
    );

    if (!result)
        return Results.NotFound();

    return Results.NoContent();
})
.RequireAuthorization("RequireAdminRole")
.WithName("UpdateChecklistTemplate")
.WithOpenApi();

app.MapDelete("/api/checklist/templates/{id}", async (int id, ChecklistService checklistService) =>
{
    var result = await checklistService.DeleteTemplateAsync(id);

    if (!result)
        return Results.NotFound();

    return Results.NoContent();
})
.RequireAuthorization("RequireAdminRole")
.WithName("DeleteChecklistTemplate")
.WithOpenApi();
// Imagens
app.MapPost("/api/orders/{orderId}/images", async (int orderId, IFormFile file, int? checklistItemId, ClaimsPrincipal user, ImageService imageService, OrderService orderService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var order = await orderService.GetOrderByIdAsync(orderId);

    if (order == null)
        return Results.NotFound();

    // Verificar permissões
    if (order.UserId != userId && !user.IsInRole("Admin") && !user.IsInRole("Supervisor"))
        return Results.Forbid();

    // Verificar se a OS está em status que permite uploads
    if (order.Status == "Approved" || order.Status == "Rejected")
        return Results.BadRequest("Não é possível adicionar imagens a uma OS já finalizada");

    if (file == null || file.Length == 0)
        return Results.BadRequest("Nenhum arquivo foi enviado");

    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (!AppConstants.AllowedImageTypes.Contains(file.ContentType) ||
        !AppConstants.AllowedImageExtensions.Contains(extension))
        return Results.BadRequest("Tipo de arquivo não permitido. Apenas JPG, JPEG e PNG são aceitos.");

    // Limitar tamanho (5MB)
    if (file.Length > AppConstants.MaxImageFileSize)
        return Results.BadRequest("Tamanho máximo do arquivo excedido (5MB)");

    var image = await imageService.SaveImageAsync(file, orderId, checklistItemId);

    if (image == null)
        return Results.BadRequest("Erro ao salvar a imagem");

    return Results.Created($"/api/orders/{orderId}/images/{image.Id}", new ImageDto
    {
        Id = image.Id,
        FileName = image.FileName,
        ContentType = image.ContentType,
        FileSize = image.FileSize,
        UploadedAt = image.UploadedAt
    });
})
.RequireAuthorization()
.WithName("UploadImage")
.WithOpenApi()
.DisableAntiforgery();

app.MapDelete("/api/images/{id}", async (int id, ClaimsPrincipal user, ImageService imageService) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Admins e supervisores podem excluir qualquer imagem
    if (user.IsInRole("Admin") || user.IsInRole("Supervisor"))
    {
        var result = await imageService.DeleteImageAsync(id, 0);

        if (!result)
            return Results.NotFound();

        return Results.NoContent();
    }

    // Outros usuários só podem excluir suas próprias imagens
    var deleteResult = await imageService.DeleteImageAsync(id, userId);

    if (!deleteResult)
        return Results.NotFound();

    return Results.NoContent();
})
.RequireAuthorization()
.WithName("DeleteImage")
.WithOpenApi();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
            timestamp = DateTime.UtcNow
        });
    });
});

app.Run();