## run

{Description}

```
{Usage}
```

{Options}

All [workload switches](#workload-id-switches) are available too to filter the instance to run.

This command will remember the last VS that was located and run. So the next time you 
can just run the same instance by simply using `vs` (since `run` is the default command 
and can be omitted).

Examples:

```
// Runs the first VS enterprise with the Xamarin/Mobile workload
vs -sku:ent -first +mobile

// Runs VS 16.4
vs -v:16.4

// Runs VS 16.5 preview
vs -v:16.4 -pre

// Runs the last VS that was run
vs
```
