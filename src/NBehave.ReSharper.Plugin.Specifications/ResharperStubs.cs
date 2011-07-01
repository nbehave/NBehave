using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;
using Rhino.Mocks;

namespace NBehave.ReSharper.Plugin.Specifications
{
    public static class ResharperStubs
    {
        public static IProjectFile StubFeatureFile()
        {
            var featureFile = MockRepository.GenerateStub<IProjectFile>();
            var project = MockRepository.GenerateStub<IProject>();
            var assemblyLocation = MockRepository.GenerateStub<IAssemblyFile>();
            assemblyLocation.Stub(_ => _.Location).Return(new FileSystemPath(@"X:\FooProject\bin\debug\FooProject.dll"));
            project.Stub(_ => _.Name).Return("FooProject");
            var location = new FileSystemPath(@"X:\FooProject\someFeature.feature");
            featureFile.Stub(_ => _.Location).Return(location);
            featureFile.Stub(_ => _.GetProject()).Return(project);
            return featureFile;
        }
        
    }
}