namespace HopFrame.Web.Admin.Generators;

public interface IGenerator<out TGeneratedType> {

    /// <summary>
    /// Compiles the generator with all specified options
    /// </summary>
    /// <returns>The compiled data structure</returns>
    TGeneratedType Compile();

}