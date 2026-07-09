## run

{Description}

```
{Usage}
```

{Options}

All [workload switches](#workload-id-switches) are available too to filter the 
instance to run, including using the `+` prefix/alias syntax.

This command will remember the last VS that was located and run. So the next time you 
can just run the same instance by simply using `dnx vs` (since `run` is the default command 
and can be omitted).

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Runs the first VS enterprise with the Maui/Mobile workload
> dnx vs -- -sku:ent -first +mobile

# Runs VS 18.7
> dnx vs -- -v:18.7

# Runs VS 18 Insiders
> dnx vs -- -v:18 -insiders

# Runs the last VS that was run
> dnx vs
```
<!-- EXAMPLES_END -->
