namespace autheticationpart.Dtos;

public record LoginDto (string UsrName , string Password);
public record RegisterDto (
    string UsrName , 
    string Password,
    string Email,
    string Department
    );

public record TokenDto (string Token , DateTime Expity);
