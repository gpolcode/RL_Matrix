using System.Diagnostics.Metrics;

namespace RLMatrix;

public static class Meters
{
    public const string MeterName = "learning";

    private static readonly Histogram<double> _actorLearningRate;
    private static readonly Histogram<double> _reward;
    private static readonly Histogram<int> _epLength;
    private static readonly Histogram<double> _actorLoss;
    private static readonly Histogram<double> _criticLoss;
    private static readonly Histogram<double> _criticLearningRate;
    private static readonly Histogram<double> _kLDivergence;
    private static readonly Histogram<double> _entropy;
    private static readonly Histogram<double> _epsilon;
    private static readonly Histogram<double> _loss;
    private static readonly Histogram<double> _learningRate;

    static Meters()
    {
        var meter = new Meter(MeterName);

        _actorLearningRate = meter.CreateHistogram<double>("actor_learning_rate");
        _reward = meter.CreateHistogram<double>("reward");
        _epLength = meter.CreateHistogram<int>("ep_length");
        _actorLoss = meter.CreateHistogram<double>("actor_loss");
        _criticLoss = meter.CreateHistogram<double>("critic_loss");
        _criticLearningRate = meter.CreateHistogram<double>("critic_learning_rate");
        _kLDivergence = meter.CreateHistogram<double>("kl_divergence");
        _entropy = meter.CreateHistogram<double>("entropy");
        _epsilon = meter.CreateHistogram<double>("epsilon");
        _loss = meter.CreateHistogram<double>("loss");
        _learningRate = meter.CreateHistogram<double>("learning_rate");
    }

    public static void UpdateActorLearningRate(double? lr)
    {
        if (lr.HasValue)
        {
            _actorLearningRate.Record(lr.Value);
        }
    }

    public static void UpdateActorLoss(double? loss)
    {
        if (loss.HasValue)
        {
            _actorLoss.Record(loss.Value);
        }
    }

    public static void UpdateCriticLearningRate(double? lr)
    {
        if (lr.HasValue)
        {
            _criticLearningRate.Record(lr.Value);
        }
    }

    public static void UpdateCriticLoss(double? loss)
    {
        if (loss.HasValue)
        {
            _criticLoss.Record(loss.Value);
        }
    }

    public static void UpdateEntropy(double? entropy)
    {
        if (entropy.HasValue)
        {
            _entropy.Record(entropy.Value);
        }
    }

    public static void UpdateReward(double reward)
    {
        _reward.Record(reward);
    }

    public static void UpdateEpisodeLength(int episodeLength)
    {
        _epLength.Record(episodeLength);
    }

    public static void UpdateEpsilon(double? epsilon)
    {
        if (epsilon.HasValue)
        {
            _epsilon.Record(epsilon.Value);
        }
    }

    public static void UpdateKLDivergence(double? kl)
    {
        if (kl.HasValue)
        {
            _kLDivergence.Record(kl.Value);
        }
    }

    public static void UpdateLearningRate(double? lr)
    {
        if (lr.HasValue)
        {
            _learningRate.Record(lr.Value);
        }
    }

    public static void UpdateLoss(double? loss)
    {
        if (loss.HasValue)
        {
            _loss.Record(loss.Value);
        }
    }
}