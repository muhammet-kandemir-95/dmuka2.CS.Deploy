# We are opening the project directory.
cd dmuka2.CS.Deploy

# After that, adding alias to .bashrc via cmd.
dotnet run --cmd "add -a"

# Then, we are going to previous directory.
cd ..

# Finally, we have to reload to use the command.
exec bash
