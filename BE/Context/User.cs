﻿namespace BE.Context;

public class User
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Username { get; set; }
	public string PasswordHash { get; set; }
	public string Role { get; set; }
}