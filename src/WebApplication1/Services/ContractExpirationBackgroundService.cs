namespace WebApplication1.Services;

//internet basically to help me implement the automatic expiration of contracts 
public class ContractExpirationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContractExpirationBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); 

    public ContractExpirationBackgroundService(
        IServiceProvider serviceProvider, 
        ILogger<ContractExpirationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Contract Expiration Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredContracts();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Contract Expiration Background Service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Contract Expiration Background Service stopped");
    }

    private async Task ProcessExpiredContracts()
    {
        using var scope = _serviceProvider.CreateScope();
        var expirationService = scope.ServiceProvider.GetRequiredService<IContractExpirationService>();
        
        try
        {
            var processedCount = await expirationService.ProcessExpiredContractsAsync();
            
            if (processedCount > 0)
            {
                _logger.LogInformation($"Background service processed {processedCount} expired contracts");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired contracts in background service");
        }
    }
} 