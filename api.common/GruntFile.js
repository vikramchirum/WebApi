module.exports = function (grunt) {

    var pkg = grunt.file.readJSON('package.json');
    var version = '';
    var solution_name = pkg.name + '.sln';

    var branch_name = grunt.option('branch_name');
    if (branch_name && branch_name !== 'dev' && branch_name !== 'master') {
        branch_name = branch_name.replace(/\\/g, "-").replace(/\//g, "-").replace(/_/g, "-");
        branch_name = branch_name.substring(0, 30);
        version = branch_name;
    }

    var octo_api_url = grunt.option('octo_api_url');
    var octo_api_key = grunt.option('octo_api_key');
    var nexus_api_key = grunt.option('nexus_api_key');
    var nexus_source = grunt.option('nexus_source');

    grunt.initConfig({	
        clean: {
            build: {
                options: {
                    force: true
                },
                src: ['.build']
            }
        },
        assemblyinfo: {
            options: {
                // Can be solutions, projects or individual assembly info files 
                files: [solution_name], 
                info: {
                    version: pkg.version,
                    fileVersion: pkg.version
                }
            }
        },		
        nugetrestore: {
            restore: {
                src: ['**/packages.config', '!**/node_modules/**', '!**/packages/**'],
                dest: 'packages/'
            }
        },
        msbuild: {
            sln: {
                src: [solution_name],
                options: {
                    projectConfiguration: 'Release',
                    targets: ['Clean', 'Rebuild'],
                    version: 4,
                    maxCpuCount: 1,
                    buildParameters: {
                        OutputPath: '../.build/temp',
                        Platform: 'Any CPU',
                        RunOctoPack: true,						
                        OctoPackUseFileVersion: true,
                        OctoPackPublishPackageToFileShare: '../.build/packages/octo',
                        OctoPackAppendToVersion: version
                    },
                    nodeReuse: true,
                    customArgs: ['/noautoresponse', '/detailedsummary'],
                    verbosity: 'normal'
                }
            }
        },		
        mstest: {
            test: {
                src: ['.build/temp/*.tests.dll'] // Points to test dll 
            }
        },
        nugetpack: {
            nuget: {
                src: ['**/api.common.csproj'], //List of csproj files to build to send to nexus as nuget packages
                dest: '.build/packages/nuget/',
                options: {
                    version: pkg.version,
                    build: true,
                    includeReferencedProjects: true
                }
            }
        },
        'octo-push': {
            options: {
                host: octo_api_url,
                apiKey: octo_api_key,
                replace: true
            },
            src: ['./.build/packages/octo/**/*']
        },
        nugetpush: {
            dist: {
                src: '.build/packages/nuget/*.nupkg',
                options: {
                    apiKey: nexus_api_key,
					source: nexus_source
                }
            }
        }
    });

    //Use one of the options below

    //Use this if the solution has only octopus projects    
    //grunt.registerTask('build', ['clean', 'assemblyinfo', 'nugetrestore', 'msbuild']);

    //Use this if the solution has nexus projects (nuget packages) even if it has octopus projects
    grunt.registerTask('build', ['clean', 'assemblyinfo', 'nugetrestore', 'msbuild', 'nugetpack']);

    require('load-grunt-tasks')(grunt);
};
