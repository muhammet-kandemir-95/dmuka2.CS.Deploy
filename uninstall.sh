# We are opening the project directory.
cd dmuka2.CS.Deploy

# After that, removing alias to .bashrc via cmd.
dotnet run --cmd "remove -a"

# Then, we are going to previous directory.
cd ..

# Finally, we have to reload to use the command.
exec bash
