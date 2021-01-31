## alias

{Description}

```
{Usage}
```

All built-in commands support a `-save:[alias]` option that will cause 
the command to be saved with that alias. From that point on, it's possible 
to just run the command (including all saved arguments) by just running 
the alias instead.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Save the first VS enterprise with the Xamarin/Mobile workload as the "mobile" alias
> vs -sku:ent -first +mobile -save:mobile

# Runs the saved alias with all the original arguments
> vs mobile
```
<!-- EXAMPLES_END -->
